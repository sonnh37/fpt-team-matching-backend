using Confluent.Kafka;
using FPT.TeamMatching.Domain.Configs;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Message;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Domain.Utilities.Redis;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using IDatabase = StackExchange.Redis.IDatabase;

namespace FPT.TeamMatching.Services;

public class MessageService : IMessageService
{
    private readonly IMongoUnitOfWork _unitOfWork;
    private readonly HttpContextAccessor _httpContextAccessor;
    private readonly IDatabase _redis;
    private readonly RedisUtil _redisUtil;
    public MessageService(IMongoUnitOfWork unitOfWork, RedisConfig redis, RedisUtil redisUtil)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor ??= new HttpContextAccessor();
        _redis = redis.GetConnection();
        _redisUtil = redisUtil;
    }

    public async Task<BusinessResult> GetAllMessageInDay(Guid conversationId)
    {
        try
        {
            if (_httpContextAccessor?.HttpContext == null ||
                !_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                return new BusinessResult(Const.FAIL_CODE, Const.FAIL_READ_MSG, "Not login yet");

            // Lấy thông tin UserId từ Claims
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return new BusinessResult(Const.FAIL_CODE, Const.FAIL_READ_MSG, "No user claim found");
            
            //1. Đọc message chưa được commit 
            var consumerConfig = new ConsumerConfig
            {
                GroupId = "chat-consumer",
                BootstrapServers = "fpt.matching.kafka:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                IsolationLevel = IsolationLevel.ReadUncommitted
            };
    
            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            consumer.Subscribe("chat.message");
            List<Message> messages = new List<Message>();
            while (true)
            {
                var consumeResult = consumer.Consume(TimeSpan.FromSeconds(2));
                if (consumeResult == null)
                    break;
                if (consumeResult.Message.Key == conversationId.ToString())
                {
                    var messageModel = JsonConvert.DeserializeObject<MessageModel>(consumeResult.Message.Value);
                    var messageEntity = new Message
                    {
                        ConversationId = consumeResult.Message.Key,
                        Content = messageModel.Message,
                        SendById = messageModel.UserId,
                        CreatedDate = messageModel.CreatedDate
                    };
                    messages.Add(messageEntity);
                }
            }

            consumer.Close();
            
            //2. Đánh dấu đã xem nếu trùng userId trong redis
            var lastMessage = await _redisUtil.GetLastMessageFromRedis(conversationId);
            if (lastMessage != null || lastMessage.SenderId.ToString() != userIdClaim)
            {
                await _redisUtil.SaveLastMessageToRedis(conversationId, lastMessage.SenderId, lastMessage.Content,
                    DateTimeOffset.Parse(lastMessage.CreatedDate.ToString()).ToUnixTimeSeconds(), "1");
            }
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, messages);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }

    public async Task<BusinessResult> GetMessageByConversationId(MessageGetAllQuery query)
    {
        try
        {
            var result = await _unitOfWork.MessageRepository.GetMessageByConversationId(query.ConversationId, query.PageNumber, query.PageSize);
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }
}
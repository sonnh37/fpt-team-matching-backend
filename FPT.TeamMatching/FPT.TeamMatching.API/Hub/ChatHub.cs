using System.Text.Json;
using AutoMapper;
using Confluent.Kafka;
using FPT.TeamMatching.Domain.Configs;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models;
using Microsoft.AspNetCore.SignalR;
using Quartz.Util;
using StackExchange.Redis;
using Task = System.Threading.Tasks.Task;

namespace FPT.TeamMatching.API.Hub;

public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly IMongoUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDatabase _redis;
    private readonly IKafkaProducerConfig _kafkaProducer;
    public ChatHub(IMongoUnitOfWork unitOfWork, IMapper mapper, RedisConfig redisConfig, IKafkaProducerConfig kafkaProducerConfig)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _redis = redisConfig.GetConnection();
        _kafkaProducer = kafkaProducerConfig;
    }
    public async Task SaveLastMessageToRedis(Guid? conversationId, Guid? senderId, string content, long timestamp)
    {
        string redisKey = $"conversation:last_message:{conversationId}";

        var lastMessageData = new HashEntry[]
        {
            new HashEntry("senderId", senderId.ToString()),
            new HashEntry("content", content),
            new HashEntry("timestamp", timestamp.ToString()),
            new HashEntry("isSeen", "0"),
        };

        await _redis.HashSetAsync(redisKey, lastMessageData);
    }

    public async Task SendMessage(string message)
    {
        try
        {
            //1. Lấy value của connectionId trong redis
            string redisKey = $"chat:{Context.ConnectionId}";
            string? redisValue = await _redis.StringGetAsync(redisKey);

            //2. Kiểm tra xem redisKey có tồn tại không
            if (redisValue.IsNullOrWhiteSpace())
            {
                Console.WriteLine("Not found redis key");
                throw new Exception("Key not found");
            }

            //3. Convert lại redis value
            var conn = JsonSerializer.Deserialize<ConversationMemberModel>(redisValue);
            //4. Bắn message vào kafka
            var messageModel = new MessageModel
            {
                Message = message,
                UserId = conn.UserId.ToString(),
                CreatedDate = DateTime.Now,
            };

            var kafkaMessage = JsonSerializer.Serialize(messageModel);
            await _kafkaProducer.ProduceAsync("chat.message", new Message<string, string>
            {
                Key = conn.ConversationId.ToString(),
                Value = kafkaMessage
            });

            //5. Send message
            await Clients.Group(conn.ConversationId.ToString())
                .SendAsync("ReceiveSpecificMessage", conn.UserId, message);
            
            //6. Lưu lại lasted message vào redis
            await SaveLastMessageToRedis(conn.ConversationId, conn.UserId, message,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task JoinSpecificChatRoom(ConversationMemberModel conn)
    {
        try
        {
            //1.1 Kiểm tra xem ConversationId có empty không
            // nếu empty có nghĩa conversation đó chưa tồn tại và cần phải gửi kèm với partnerId
            // để tạo ra conversation và conversation member
            if (conn.ConversationId == null)
            {
                // Tạo conversation mới
                var conversationEntity = new Conversation
                {
                    ConversationName = "",
                };
                _unitOfWork.ConversationRepository.Add(conversationEntity);
                
                // Tạo conversation member
                var conversationUser = new ConversationMember
                {
                    ConversationId = conversationEntity.Id,
                    UserId = conn.UserId.ToString(),
                };
                _unitOfWork.ConversationMemberRepository.Add(conversationUser);

                var conversationPartner = new ConversationMember
                {
                    ConversationId = conversationEntity.Id,
                    UserId = conn.PartnerId.ToString(),
                };
                _unitOfWork.ConversationMemberRepository.Add(conversationPartner);
                // Gắn conversation ID vào cho request lại
                conn.ConversationId = Guid.Parse(conversationEntity.Id);
            }

            //1.2 Thêm vào SignalR group
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.ConversationId.ToString());

            //2. Lưu vào redis ConnectionId, JSON ConversationMemberModel
            string redisKey = $"chat:{Context.ConnectionId}";
            string redisValue = JsonSerializer.Serialize(conn);

            await _redis.StringSetAsync(redisKey, redisValue);

            //thông báo test
            await Clients.Group(conn.ConversationId.ToString()).SendAsync("JoinSpecificChatRoom", "admin",
                $"{conn.UserId} has joined {conn.ConversationId}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    //Custom lại khi ngắt kết nối thì xóa key trong redis rồi mới tiếp tục xóa như default
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            //1. Xóa key trong redis
            string redisKey = $"chat:{Context.ConnectionId}";
            await _redis.KeyDeleteAsync(redisKey);

        }
        catch (Exception e)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
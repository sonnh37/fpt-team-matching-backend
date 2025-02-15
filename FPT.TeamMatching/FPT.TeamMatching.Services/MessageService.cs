using Confluent.Kafka;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Utilities;
using Newtonsoft.Json;

namespace FPT.TeamMatching.Services;

public class MessageService : IMessageService
{
    private readonly IMongoUnitOfWork _unitOfWork;

    public MessageService(IMongoUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BusinessResult> GetAllMessageInDay(Guid conversationId)
    {
        try
        {
            var consumerConfig = new ConsumerConfig
            {
                GroupId = "chat-consumer",
                BootstrapServers = "localhost:29092",
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
                        // Id = 
                    };
                    messages.Add(messageEntity);
                }
            }

            consumer.Close();
            return new BusinessResult(Const.SUCCESS_CODE, Const.SUCCESS_READ_MSG, messages);
        }
        catch (Exception e)
        {
            return new BusinessResult(Const.FAIL_CODE, e.Message);
        }
    }
}
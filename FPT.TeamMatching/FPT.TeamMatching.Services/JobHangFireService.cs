using Confluent.Kafka;
using FPT.TeamMatching.Domain.Contracts.Hangfire;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models;
using Newtonsoft.Json;
using Task = System.Threading.Tasks.Task;

namespace FPT.TeamMatching.Services;

public class JobHangFireService : IJobHangfireService
{
    private readonly IMongoUnitOfWork _unitOfWork;
    public JobHangFireService(IMongoUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public void FireAndForgetJob()
    {   
        Console.WriteLine("FireAndForgetJob");
    }

    public async Task ReccuringJob()
    {
        var consumerConfig = new ConsumerConfig
        {
            GroupId = "chat-consumer",
            BootstrapServers = "localhost:29092",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false 
        };

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe("chat.message");

        try
        {
            while (true)
            {
                var consumeResult = consumer.Consume(TimeSpan.FromSeconds(2)); 

                if (consumeResult == null) 
                    break; 

                Console.WriteLine("Received a message");

                var message = JsonConvert.DeserializeObject<MessageModel>(consumeResult.Message.Value);
                var conversationId = consumeResult.Message.Key;

                var newMessage = new Message
                {
                    ConversationId = conversationId,
                    Content = message.Message,
                    SendById = message.UserId,
                    CreatedDate = message.CreatedDate,
                };

                _unitOfWork.MessageRepository.AddMessage(newMessage);
                 await _unitOfWork.SaveChanges();

                  consumer.Commit(consumeResult);
            }

        }
        catch (ConsumeException ex)
        {
            Console.WriteLine($"Kafka consume error: {ex.Error.Reason}");
        }
        finally
        {
            consumer.Close();
            Console.WriteLine("Finished consuming messages.");
        }
    }


    public void DelayedJob()
    {
        throw new NotImplementedException();
    }

    public void ContinuationJob()
    {
        throw new NotImplementedException();
    }
}
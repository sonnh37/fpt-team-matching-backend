using Confluent.Kafka;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace FPT.TeamMatching.Domain.Configs;

public class MessageConsumer(IMongoUnitOfWork unitOfWork) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => { _ = ConsumeAsync("chat.message", stoppingToken); }, stoppingToken);
    }

    public async Task ConsumeAsync(string topic, CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = "message-group",
            BootstrapServers = "fpt.matching.kafka:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);

            var message = JsonConvert.DeserializeObject<Message>(consumeResult.Message.Value);
            unitOfWork.MessageRepository.AddMessage(message);
        }

        consumer.Close();
    }
}
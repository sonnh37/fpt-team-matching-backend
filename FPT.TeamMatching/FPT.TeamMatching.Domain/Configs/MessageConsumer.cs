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
        return Task.Run(() => { _ = ConsumeAsync("order-topic", stoppingToken); }, stoppingToken);
    }

    public async Task ConsumeAsync(string topic, CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = "message-group",
            BootstrapServers = "localhost:29092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);

            var message = JsonConvert.DeserializeObject<Message>(consumeResult.Message.Value);
            unitOfWork.MessageRepository.AddMessage(message);
            // using var scope = scopeFactory.CreateScope();
            // var dbContext = scope.ServiceProvider.GetRequiredService<InvoiceDbContext>();
            //
            // var invoiceEntity = new InvoiceModel
            // {
            //     Id = (Guid)order.InvoiceId,
            //     PaymentAmount = order.TotalPrice,
            //     PaymentDate = DateTime.Now,
            // };
            // dbContext.Invoice.Add(invoiceEntity);
            // await dbContext.SaveChangesAsync();
        }

        consumer.Close();
    }
}
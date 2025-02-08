using Confluent.Kafka;

namespace FPT.TeamMatching.Domain.Lib;
public interface IKafkaProducerConfig
{
    Task ProduceAsync(string topic, Message<string, string> message);
}
public class KafkaProducer : IKafkaProducerConfig
{
    private readonly IProducer<string, string> _producer;
    public KafkaProducer()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:29092",
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }
    public async Task ProduceAsync(string topic, Message<string, string> message)
    {
        await _producer.ProduceAsync(topic, message);
    }
}
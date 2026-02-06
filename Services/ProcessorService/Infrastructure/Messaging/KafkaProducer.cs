using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace SoftoriaTestTask.Services.ProcessorService.Infrastructure.Messaging;

public class KafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic = "coin-prices-stream";

    public KafkaProducer(IConfiguration config)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"] ?? "kafka:29092",
            Acks = Acks.All
        };
        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public async Task PublishAsync(string key, string payload)
    {try
        {
            var result = await _producer.ProduceAsync(_topic, new Message<string, string>
            {
                Key = key,
                Value = payload
            });
        }
        catch (ProduceException<string, string> e)
        {
            throw new Exception($"Kafka failure: {e.Error.Reason}");
        }
    }
}

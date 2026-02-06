using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace SoftoriaTestTask.Services.ConsumerService.Infrastructure.Messaging;

public class KafkaConsumer : IDisposable
{
    private readonly IConsumer<string, string> _consumer;
    private readonly string _topic;

    public KafkaConsumer(IConfiguration config)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"] ?? "kafka:29092",
            GroupId = config["Kafka:GroupId"] ?? "softoriatesttask-consumers",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        _topic = config["Kafka:Topic"] ?? "coin-prices-stream";
        _consumer.Subscribe(_topic);
    }

    public ConsumeResult<string, string> Consume(CancellationToken ct) => _consumer.Consume(ct);

    public void Commit(ConsumeResult<string, string> result) => _consumer.Commit(result);

    public void Dispose() => _consumer.Dispose();
}
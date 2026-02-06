using System.Text.Json;
using SoftoriaTestTask.Services.ConsumerService.Infrastructure.Messaging;
using SoftoriaTestTask.Shared.Domain.Models;
using SoftoriaTestTask.Shared.Infrastructure.Persistence;

namespace SoftoriaTestTask.Services.ConsumerService.Worker.Workers;

public class CoinStorageWorker : BackgroundService
{
    private readonly KafkaConsumer _kafka;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CoinStorageWorker> _logger;

    public CoinStorageWorker(
        KafkaConsumer kafka,
        IServiceProvider serviceProvider,
        ILogger<CoinStorageWorker> logger)
    {
        _kafka = kafka;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consumer: Waiting for messages from Kafka...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = _kafka.Consume(stoppingToken);
                if (result == null) continue;

                var coinDataBatch = JsonSerializer.Deserialize<List<CoinData>>(result.Message.Value);
                if (coinDataBatch == null) continue;
                
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var priceRecords = coinDataBatch.Select(c => new CoinDataDto()
                    {
                        Rank = c.Rank,
                        Symbol = c.Symbol,
                        Name = c.Name,
                        Price = c.Price,
                        MarketCap = c.MarketCap,
                        Volume24h = c.Volume24h,
                        Change24h = c.Change24h,
                    });

                    await db.CoinPrices.AddRangeAsync(priceRecords, stoppingToken);
                    await db.SaveChangesAsync(stoppingToken);

                    _kafka.Commit(result);
                    _logger.LogInformation("Successfully persisted {Count} coins to PostgreSQL", coinDataBatch.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process Kafka message batch");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}

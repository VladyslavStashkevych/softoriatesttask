using Microsoft.EntityFrameworkCore;
using SoftoriaTestTask.Services.ProcessorService.Infrastructure.Messaging;
using SoftoriaTestTask.Shared.Infrastructure.Persistence;

namespace SoftoriaTestTask.Services.ProcessorService.Worker.Workers;

public class OutboxProcessorWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly KafkaProducer _kafka;
    private readonly ILogger<OutboxProcessorWorker> _logger;
    
    public OutboxProcessorWorker(
        KafkaProducer kafka,
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessorWorker> logger)
    {
        _kafka = kafka;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Processor started. Monitoring SQLite for new batches...");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var messages = await dbContext.OutboxMessages
                        .Where(m => !m.IsProcessed)
                        .OrderBy(m => m.OccurredOn)
                        .Take(5)
                        .ToListAsync(stoppingToken);

                    if (messages.Any())
                    {
                        foreach (var message in messages)
                        {
                            try
                            {
                                _logger.LogInformation("Processing message {Id}", message.Id);

                                await _kafka.PublishAsync(message.Id.ToString(), message.Payload);
                                message.IsProcessed = true;

                                _logger.LogInformation("Pushed batch {Id} to Kafka", message.Id);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Kafka delivery failed for message {Id}", message.Id);
                            }
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error in Outbox Polling Loop");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}

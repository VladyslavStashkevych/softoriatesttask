using Microsoft.EntityFrameworkCore;
using SoftoriaTestTask.Shared.Infrastructure.Persistence;

namespace SoftoriaTestTask.Services.ProcessorService.Worker;

public class OutboxProcessorWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorWorker> _logger;
    
    public OutboxProcessorWorker(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessorWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                var messages = await dbContext.OutboxMessages
                    .Where(m => !m.IsProcessed)
                    .OrderBy(m => m.OccurredOn)
                    .Take(10)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try 
                    {
                        _logger.LogInformation("Processing message {Id}", message.Id);
                        
                        // 2. TODO: Send to Kafka here
                        
                        message.IsProcessed = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process message {Id}", message.Id);
                    }
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}

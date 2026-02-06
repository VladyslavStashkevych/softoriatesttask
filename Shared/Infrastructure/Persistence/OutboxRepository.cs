using System.Text.Json;
using SoftoriaTestTask.Shared.Domain.Interfaces;
using SoftoriaTestTask.Shared.Domain.Models;

namespace SoftoriaTestTask.Shared.Infrastructure.Persistence;

public class OutboxRepository : IOutboxRepository
{
    private readonly AppDbContext _context;

    public OutboxRepository(AppDbContext context) => _context = context;

    public async Task SaveBatchAsync(List<CoinData> batch)
    {
        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Payload = JsonSerializer.Serialize(batch), // Store as JSON for Kafka later
            OccurredOn = DateTime.UtcNow,
            IsProcessed = false
        };

        await _context.OutboxMessages.AddAsync(message);
        await _context.SaveChangesAsync();
    }
}
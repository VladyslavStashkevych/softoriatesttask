using Microsoft.EntityFrameworkCore;
using SoftoriaTestTask.Shared.Domain.Models;

namespace SoftoriaTestTask.Shared.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<CoinDataDto> CoinPrices => Set<CoinDataDto>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Payload).IsRequired();
        });
        
        modelBuilder.Entity<CoinDataDto>().HasKey(c => c.Id);
    }
}
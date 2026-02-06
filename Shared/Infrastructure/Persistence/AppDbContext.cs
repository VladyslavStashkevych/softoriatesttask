using Microsoft.EntityFrameworkCore;
using SoftoriaTestTask.Shared.Domain.Models;

namespace SoftoriaTestTask.Shared.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define your schema here, such as setting the ID and Payload types
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Payload).IsRequired();
        });
    }
}
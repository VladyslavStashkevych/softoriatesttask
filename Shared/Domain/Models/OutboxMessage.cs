namespace SoftoriaTestTask.Shared.Domain.Models;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Payload { get; set; } = string.Empty;
    public DateTime OccurredOn { get; set; }
    public bool IsProcessed { get; set; }
}
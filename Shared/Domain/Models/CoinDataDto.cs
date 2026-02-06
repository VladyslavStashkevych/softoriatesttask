namespace SoftoriaTestTask.Shared.Domain.Models;

public class CoinDataDto
{
    public Guid Id { get; set; } = Guid.NewGuid(); 
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    
    public int Rank { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal MarketCap { get; set; }
    public decimal Volume24h { get; set; }
    public decimal Change24h { get; set; }
}
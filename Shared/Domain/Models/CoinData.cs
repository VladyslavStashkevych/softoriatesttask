namespace SoftoriaTestTask.Shared.Domain.Models;

public record struct CoinData
{
    public int Rank { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public decimal Price { get; set; }
    public decimal MarketCap { get; set; }
    public decimal Volume24h { get; set; }
    public decimal Change24h { get; set; }
}

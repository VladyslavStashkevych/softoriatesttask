using SoftoriaTestTask.Shared.Domain.Models;

namespace SoftoriaTestTask.Services.ParserService.Domain.Models;

public record struct LoopStepResult
{
    public CoinData[] Batch { get; set; }
    public int NextState { get; set; }
}
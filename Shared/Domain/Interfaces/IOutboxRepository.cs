using SoftoriaTestTask.Shared.Domain.Models;

namespace SoftoriaTestTask.Shared.Domain.Interfaces;

public interface IOutboxRepository
{
    public Task SaveBatchAsync(List<CoinData> batch);
}
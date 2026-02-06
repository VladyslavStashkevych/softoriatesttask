using MediatR;
using SoftoriaTestTask.Shared.Domain.Interfaces;

namespace SoftoriaTestTask.Services.ParserService.Application.Commands;

public class IngestBatchHandler : IRequestHandler<IngestBatchCommand, bool>
{
    private readonly IOutboxRepository _repository;

    public IngestBatchHandler(IOutboxRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(IngestBatchCommand request, CancellationToken cancellationToken)
    {
        if (request.Batch == null || !request.Batch.Any()) return false;

        await _repository.SaveBatchAsync(request.Batch);

        return true;
    }
}
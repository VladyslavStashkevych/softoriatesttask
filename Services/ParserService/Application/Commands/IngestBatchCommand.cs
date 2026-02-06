using MediatR;
using SoftoriaTestTask.Shared.Domain.Models;

namespace SoftoriaTestTask.Services.ParserService.Application.Commands;

public record IngestBatchCommand(List<CoinData> Batch) : IRequest<bool>;

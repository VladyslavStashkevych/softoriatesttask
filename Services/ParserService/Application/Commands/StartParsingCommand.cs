using MediatR;

namespace SoftoriaTestTask.Services.ParserService.Application.Commands;

public record StartParsingCommand() : IRequest<string>;
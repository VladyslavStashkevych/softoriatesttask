using MediatR;
using SoftoriaTestTask.Services.ParserService.Domain.Interfaces;

namespace SoftoriaTestTask.Services.ParserService.Application.Commands;

public class StartParsingHandler : IRequestHandler<StartParsingCommand, string>
{
    private readonly IParserService _parserService;

    public StartParsingHandler(IParserService parserService)
    {
        _parserService = parserService;
    }

    public async Task<string> Handle(StartParsingCommand request, CancellationToken cancellationToken)
    {
        // This calls the 'ExecuteScrapeAsync' method we built earlier
        return await _parserService.ExecuteParseAsync();
    }
}
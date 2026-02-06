namespace SoftoriaTestTask.Services.ParserService.Domain.Interfaces;

public interface IParserService
{
    Task<string> ExecuteParseAsync();
}
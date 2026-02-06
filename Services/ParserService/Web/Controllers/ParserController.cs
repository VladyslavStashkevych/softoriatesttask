using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoftoriaTestTask.Services.ParserService.Application.Commands;

namespace SoftoriaTestTask.Services.ParserService.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParserController : ControllerBase
{
    private readonly IMediator _mediator;

    public ParserController(IMediator mediator) => _mediator = mediator;

    [HttpPost("start")]
    public async Task<IActionResult> StartScrape()
    {
        var result = await _mediator.Send(new StartParsingCommand());
        return Ok(result);
    }
}
using System.Text;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftoriaTestTask.Services.ApiService.Web.Models;
using SoftoriaTestTask.Shared.Infrastructure.Persistence;

namespace SoftoriaTestTask.Services.ApiService.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoinsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CoinsController(AppDbContext context) => _context = context;

    [HttpPost("search")]
    public async Task<IActionResult> SearchPrices([FromBody] CoinSearchRequest request)
    {
        var query = _context.CoinPrices.AsNoTracking().AsQueryable();

        try
        {
            if (!string.IsNullOrWhiteSpace(request.Filters))
            {
                query = query.Where(request.Filters);
            }

            if (!string.IsNullOrWhiteSpace(request.Sorting))
            {
                query = query.OrderBy(request.Sorting);
            }
            else
            {
                query = query.OrderBy(c => c.Rank);
            }
        }
        catch (ParseException ex)
        {
            // Return 400 if the filter/sort string is invalid
            return BadRequest(new { error = "Invalid filter or sort expression", details = ex.Message });
        }

        if (!string.IsNullOrWhiteSpace(request.SearchAfterToken))
        {
            var lastRank = DecodeToken(request.SearchAfterToken);
            query = query.Where(c => c.Rank > lastRank);
        }

        var items = await query.Skip(request.Offset).Take(request.Limit).ToListAsync();
    
        return Ok(new { 
            data = items, 
            search_after_token = items.Any() ? EncodeToken(items.Last().Rank) : null 
        });
    }

    private string EncodeToken(int rank) => Convert.ToBase64String(Encoding.UTF8.GetBytes(rank.ToString()));
    private int DecodeToken(string token) => int.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(token)));
}
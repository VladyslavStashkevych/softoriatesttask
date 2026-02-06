using MediatR;
using Microsoft.Playwright;
using SoftoriaTestTask.Services.ParserService.Application.Commands;
using SoftoriaTestTask.Services.ParserService.Domain.Interfaces;
using SoftoriaTestTask.Services.ParserService.Domain.Models;
using SoftoriaTestTask.Services.ParserService.Infrastructure.Constants;
using SoftoriaTestTask.Shared.Domain.Models;

namespace SoftoriaTestTask.Services.ParserService.Infrastructure;

public class ParserService : IParserService, IAsyncDisposable
{
    private IPage? _page;
    private IBrowser _browser;
    private IBrowserContext? _context;
    private readonly IMediator _mediator;

    public ParserService(IBrowser browser, IMediator mediator)
    {
        _browser = browser;
        _mediator = mediator;
    }

    public async Task<string> ExecuteParseAsync()
    {
        var maxProcessedRank = 0;
        var emptyAttempts = 0;
        var currentBatch = new List<CoinData>(200);

        try
        {
            await this.InitializeAsync();
            await _page.GotoAsync("https://coinmarketcap.com/all/views/all/");

            while (emptyAttempts <= 3)
            {
                var result = await _page.EvaluateAsync<LoopStepResult>(ScraperScripts.ParseAndGetStateScript);

                maxProcessedRank = this.TryAddBatch(currentBatch, result.Batch, maxProcessedRank);

                // check state of page to decide next action
                // recycle if 2000 rows processed, when works
                // var state = (maxProcessedRank > 0 && maxProcessedRank % 2000 == 0) ? 3 : result.NextState;
                var state = result.NextState;

                // action: 3 = Recycle, 2 = Load More, 1 = Wait/Scroll, 0 = Done
                emptyAttempts = await this.MakeActionOnPageStateAsync(state, maxProcessedRank, currentBatch)
                    ? 0
                    : emptyAttempts + 1;
            }

            return $"Success: {maxProcessedRank} coins captured.";
        }
        // TODO: make more exceptions
        catch (Exception ex)
        {
            return $"Failed at rank {maxProcessedRank}: {ex.Message}";
        }
        finally
        {
            await this.DisposeAsync();
        }
    }

    public async Task InitializeAsync()
    {
        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1920, Height = 5000 }
        });

        _page = await _context.NewPageAsync();

        // Traffic minimization logic
        var blocked = new[] { "image", "media", "font", "stylesheet", "other" };
        await _page.RouteAsync("**/*", async route =>
        {
            var type = route.Request.ResourceType;
            
            if (blocked.Contains(type)) await route.AbortAsync();
            else await route.ContinueAsync();
        });
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_page != null) await _page.CloseAsync();
        if (_context != null)
        {
            await _context.CloseAsync();
            await _context.DisposeAsync();
        }
        
        GC.Collect(0);
        GC.SuppressFinalize(this);
    }
    
    private int TryAddBatch(List<CoinData> currentBatch, CoinData[] batch, int maxProcessedRank)
    {
        foreach (var coin in batch)
        {
            if (maxProcessedRank < coin.Rank)
            {
                currentBatch.Add(coin);
                maxProcessedRank = coin.Rank;
            }
        }
        
        return maxProcessedRank;
    }
    
    private async Task<bool> MakeActionOnPageStateAsync(int pageState, int maxProcessedRank, List<CoinData> currentBatch)
    {
        switch (pageState)
        {
            case 1:
                await _page.EvaluateAsync("() => document.querySelector('tr.cmc-table-row')?.scrollIntoView()");
                await _page.WaitForFunctionAsync(
                    "() => document.querySelector('tr.cmc-table-row td')?.textContent.trim() !== ''");
                
                return true;

            case 2:
                var loadMore = _page.GetByRole(AriaRole.Button, new() { Name = "Load More" });
                await loadMore.ScrollIntoViewIfNeededAsync();

                await loadMore.ClickAsync(new() { Force = true });
                await _page.WaitForFunctionAsync(ScraperScripts.WaitForNewRowsScript);

                // send saving to database
                await _mediator.Send(new IngestBatchCommand(currentBatch));
                currentBatch = new List<CoinData>(200);
                
                return true;

            case 3:
                Console.WriteLine($"Recycling session at rank {maxProcessedRank}...");
                // TODO: fix later (fast-forward breaks the page state)
                // recycling to clean up memory
                // await this.RecycleSessionAsync(maxProcessedRank);

                return true;

            case 0:
                await Task.Delay(500);
                
                break;
        }
        
        // if we are here, then current batch is either empty or it has last <200 elements
        await _mediator.Send(new IngestBatchCommand(currentBatch));
        currentBatch = new List<CoinData>(200);

        return false;
    }
    
    // to be used, when fixed
    private async Task RecycleSessionAsync(int currentMaxRank)
    {
        if (_page != null) await _page.CloseAsync();
        if (_context != null) await _context.DisposeAsync();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        await InitializeAsync();

        await _page.GotoAsync("https://coinmarketcap.com/all/views/all/");

        await FastForwardToRankAsync(currentMaxRank);
    }
    
    private async Task FastForwardToRankAsync(int targetRank)
    {
        // Execute the script with a long timeout because this is a heavy operation
        await _page.EvaluateAsync(ScraperScripts.FastForwardScript, targetRank);
    
        // After jumping, clear the DOM of the thousands of rows we just loaded
        // but don't need to process again.
        await _page.EvaluateAsync(@"() => {
            const rows = document.querySelectorAll('tr.cmc-table-row');
            rows.forEach(r => r.remove());
        }");
    }
}
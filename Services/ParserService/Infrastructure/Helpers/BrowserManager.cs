using Microsoft.Playwright;

namespace SoftoriaTestTask.Services.ParserService.Infrastructure.Helpers;

public class BrowserManager : IAsyncDisposable
{
    private IBrowser? _browser;
    private IPage? _page;

    public async Task<IPage> GetPageAsync()
    {
        if (_browser == null)
        {
            var playwright = await Playwright.CreateAsync();
            _browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    Devtools = false,
                    Args =
                    [
                        "--disable-dev-shm-usage", "--disable-gpu", "--no-sandbox",
                        "--single-process", "--disable-extensions"
                    ]

                });
            
            var context = await _browser.NewContextAsync(
                new BrowserNewContextOptions
                {
                    ViewportSize = new ViewportSize { Width = 1920, Height = 5000 }
                });
            
            _page = await context.NewPageAsync();
        }
        
        return _page;
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser != null) await _browser.DisposeAsync();
    }
}
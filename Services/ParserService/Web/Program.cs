using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using SoftoriaTestTask.Services.ParserService.Application.Commands;
using SoftoriaTestTask.Services.ParserService.Domain.Interfaces;
using SoftoriaTestTask.Services.ParserService.Web.Middleware;
using SoftoriaTestTask.Shared.Domain.Interfaces;
using SoftoriaTestTask.Shared.Infrastructure.Persistence;

namespace SoftoriaTestTask.Services.ParserService.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
                              ?? "Data Source=outbox.db"));
        
        builder.Services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(StartParsingCommand).Assembly));

        builder.Services.AddControllers();
        
        builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
        builder.Services.AddScoped<IParserService, Infrastructure.ParserService>();
        builder.Services.AddSingleton<IPlaywright>(sp => Playwright.CreateAsync().GetAwaiter().GetResult());
        builder.Services.AddSingleton<IBrowser>(sp => 
        {
            var playwright = sp.GetRequiredService<IPlaywright>();
            return playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Devtools = false,
                Args =
                [
                    "--disable-dev-shm-usage", "--disable-gpu", "--no-sandbox", 
                    "--single-process", "--disable-extensions"
                ]
            }).GetAwaiter().GetResult();
        });
        
        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }
        
        app.UseMiddleware<ExceptionHandler>();
        
        app.MapControllers();
        app.MapGet("/", () => "Hello World!");

        app.Run();
    }
}
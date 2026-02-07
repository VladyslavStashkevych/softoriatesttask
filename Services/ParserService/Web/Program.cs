using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using SoftoriaTestTask.Services.ParserService.Application.Commands;
using SoftoriaTestTask.Services.ParserService.Domain.Interfaces;
using SoftoriaTestTask.Services.ParserService.Infrastructure.Helpers;
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
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(StartParsingCommand).Assembly));

        builder.Services.AddControllers();
        
        builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
        builder.Services.AddScoped<IParserService, Infrastructure.ParserService>();
        builder.Services.AddSingleton<BrowserManager>();
        
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
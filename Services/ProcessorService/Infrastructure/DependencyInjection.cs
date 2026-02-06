using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoftoriaTestTask.Services.ProcessorService.Infrastructure.Messaging;
using SoftoriaTestTask.Shared.Infrastructure.Persistence;

namespace SoftoriaTestTask.Services.ProcessorService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddProcessorInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddSingleton<KafkaProducer>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}
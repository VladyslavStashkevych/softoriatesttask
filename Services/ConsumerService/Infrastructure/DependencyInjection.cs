using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoftoriaTestTask.Services.ConsumerService.Infrastructure.Messaging;
using SoftoriaTestTask.Shared.Infrastructure.Persistence;

namespace SoftoriaTestTask.Services.ConsumerService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddConsumerInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<KafkaConsumer>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}
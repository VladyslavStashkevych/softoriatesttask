using SoftoriaTestTask.Services.ConsumerService.Infrastructure;
using SoftoriaTestTask.Services.ConsumerService.Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddConsumerInfrastructure(builder.Configuration);
builder.Services.AddHostedService<CoinStorageWorker>();

var host = builder.Build();
host.Run();

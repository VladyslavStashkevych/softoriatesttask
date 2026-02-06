using SoftoriaTestTask.Services.ProcessorService.Infrastructure;
using SoftoriaTestTask.Services.ProcessorService.Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddProcessorInfrastructure(builder.Configuration);
builder.Services.AddHostedService<OutboxProcessorWorker>();

var host = builder.Build();
host.Run();

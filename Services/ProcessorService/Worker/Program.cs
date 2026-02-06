using SoftoriaTestTask.Services.ProcessorService.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<OutboxProcessorWorker>();

var host = builder.Build();
host.Run();

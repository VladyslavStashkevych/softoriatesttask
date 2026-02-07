using SoftoriaTestTask.Services.ApiService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApiInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapControllers();

app.Run();

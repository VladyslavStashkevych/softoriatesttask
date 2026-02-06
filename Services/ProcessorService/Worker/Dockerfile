FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/ProcessorService/Infrastructure/SoftoriaTestTask.Services.ProcessorService.Infrastructure.csproj", "Services/ProcessorService/Infrastructure/"]
COPY ["Services/ProcessorService/Worker/SoftoriaTestTask.Services.ProcessorService.Worker.csproj", "Services/ProcessorService/Worker/"]

RUN dotnet restore "Services/ProcessorService/Worker/SoftoriaTestTask.Services.ProcessorService.Worker.csproj"
COPY . .
RUN dotnet publish "Services/ProcessorService/Worker/SoftoriaTestTask.Services.ProcessorService.Worker.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

RUN mkdir -p /app/data
RUN chmod 755 /app/data
ENTRYPOINT ["dotnet", "SoftoriaTestTask.Services.ProcessorService.Worker.dll"]
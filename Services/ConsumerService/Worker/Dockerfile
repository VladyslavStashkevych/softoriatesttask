FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Shared/Domain/SoftoriaTestTask.Shared.Domain.csproj", "Shared/Shared.Domain/"]
COPY ["Shared/Infrastructure/SoftoriaTestTask.Shared.Infrastructure.csproj", "Shared/Shared.Infrastructure/"]

# Copy Consumer Projects
COPY ["Services/ConsumerService/Worker/SoftoriaTestTask.Services.ConsumerService.Worker.csproj", "Services/ConsumerService/Worker/"]
COPY ["Services/ConsumerService/Infrastructure/SoftoriaTestTask.Services.ConsumerService.Infrastructure.csproj", "Services/ConsumerService/Infrastructure/"]

RUN dotnet restore "Services/ConsumerService/Worker/SoftoriaTestTask.Services.ConsumerService.Worker.csproj"
COPY . .

RUN dotnet publish "Services/ConsumerService/Worker/SoftoriaTestTask.Services.ConsumerService.Worker.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# No 'data' volume needed here since Postgres is a separate container
ENTRYPOINT ["dotnet", "SoftoriaTestTask.Services.ConsumerService.Worker.dll"]
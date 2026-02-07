FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Shared/Domain/SoftoriaTestTask.Shared.Domain.csproj", "Shared/Domain/"]
COPY ["Shared/Infrastructure/SoftoriaTestTask.Shared.Infrastructure.csproj", "Shared/Infrastructure/"]

COPY ["Services/ApiService/Web/SoftoriaTestTask.Services.ApiService.Web.csproj", "Services/ApiService/Web/"]
COPY ["Services/ApiService/Infrastructure/SoftoriaTestTask.Services.ApiService.Infrastructure.csproj", "Services/ApiService/Infrastructure/"]

RUN dotnet restore "Services/ApiService/Web/SoftoriaTestTask.Services.ApiService.Web.csproj"
COPY . .

RUN dotnet publish "Services/ApiService/Web/SoftoriaTestTask.Services.ApiService.Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "SoftoriaTestTask.Services.ApiService.Web.dll"]
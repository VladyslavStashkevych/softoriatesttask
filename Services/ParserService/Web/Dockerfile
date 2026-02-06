# .NET 8.0
FROM mcr.microsoft.com/playwright/dotnet:v1.41.2 AS build
WORKDIR /src

# restore dependencies
COPY ["Services/ParserService/Web/SoftoriaTestTask.Services.ParserService.Web.csproj", "Services/ParserService/Web/"]
COPY ["Services/ParserService/Application/SoftoriaTestTask.Services.ParserService.Application.csproj", "Services/ParserService/Application/"]
COPY ["Services/ParserService/Infrastructure/SoftoriaTestTask.Services.ParserService.Infrastructure.csproj", "Services/ParserService/Infrastructure/"]
COPY ["Services/ParserService/Domain/SoftoriaTestTask.Services.ParserService.Domain.csproj", "Services/ParserService/Domain/"]
RUN dotnet restore "Services/ParserService/Web/SoftoriaTestTask.Services.ParserService.Web.csproj"

# copy and build
COPY . .
WORKDIR "/src/Services/ParserService/Web"
RUN dotnet publish "SoftoriaTestTask.Services.ParserService.Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/playwright/dotnet:v1.41.2
WORKDIR /app
COPY --from=build /app/publish .

ENV PLAYWRIGHT_BROWSERS_PATH=/ms-playwright

# sqlite dir
RUN mkdir -p /app/data && chmod 777 /app/data
ENTRYPOINT ["dotnet", "SoftoriaTestTask.Services.ParserService.Web.dll"]
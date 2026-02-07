namespace SoftoriaTestTask.Services.ApiService.Web.Models;

public record CoinSearchRequest(
    int Offset = 0,
    int Limit = 100,
    string? Filters = null,
    string? Sorting = null,
    string? SearchAfterToken = null
);
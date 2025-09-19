using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;

namespace ModernizedApp.Services;

public class ExternalApiService : IExternalApiService
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;

    public ExternalApiService(HttpClient http, IMemoryCache cache)
    {
        _http = http;
        _cache = cache;
        _http.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("ModernizedApp/1.0");
    }

    public async Task<object> GetTodoAsync()
    {
        const string cacheKey = "todo-1";
        if (_cache.TryGetValue(cacheKey, out object? cached) && cached is not null)
            return cached;

        var resp = await _http.GetFromJsonAsync<object>("/todos/1");
        _cache.Set(cacheKey, resp!, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
        });
        return resp!;
    }
}

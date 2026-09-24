using System.Text.Json;
using AssetManagementSystem.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace AssetManagementSystem.Application.Services;

public sealed class CacheService : ICacheService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;

    private static readonly AsyncCircuitBreakerPolicy CircuitBreaker = Policy
        .Handle<Exception>(exception => exception is not OperationCanceledException)//per arsye se handleexception kap edhe anulimin kur klienti e anullon, dhe nuk duhet te bejme circuit breaker per kete
        .CircuitBreakerAsync(
            exceptionsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(10));

    public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
       /* return await ExecuteSafeAsync(async () =>
        {
            var json = await _cache.GetStringAsync(key, cancellationToken);
            return json is null ? default : JsonSerializer.Deserialize<T>(json, SerializerOptions);
        }, key, default(T));*/

        try
        {
            return await CircuitBreaker.ExecuteAsync(async () =>
            {
                var json = await _cache.GetStringAsync(key, cancellationToken);
                return json is null ? default : JsonSerializer.Deserialize<T>(json, SerializerOptions);
            });
        }
        catch (BrokenCircuitException)
        {
            // Redis i rene nuk eshte gabim aplikacioni: sillu sikur vlera s'ishte ne cache.
            _logger.LogWarning("Circuit Breaker is OPEN. Redis is ignored for key {Key}.", key);
            return default;
        }
        catch (Exception exception)
        {
            // Kur jemi në fazën e 3 dështimeve të para
            _logger.LogWarning(exception, "Cache read failed for key {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        await ExecuteSafeAsync(async () =>
        {
            var json = JsonSerializer.Serialize(value, SerializerOptions);
            await _cache.SetStringAsync(
                key,
                json,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl },
                cancellationToken);
        }, key);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await ExecuteSafeAsync(async () =>
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }, key);
    }

    private async Task<T?> ExecuteSafeAsync<T>(Func<Task<T?>> action, string key, T? defaultValue = default)
    {
        try
        {
            return await CircuitBreaker.ExecuteAsync(action);
        }
        catch (BrokenCircuitException)
        {
            _logger.LogWarning("Circuit Breaker is OPEN. Redis operation skipped for key {Key}.", key);
            return defaultValue;
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Cache operation failed for key {Key}", key);
            return defaultValue;
        }
    }  

    private async Task ExecuteSafeAsync(Func<Task> action, string key)
    {
        await ExecuteSafeAsync<bool>(async () =>
        {
            await action();
            return true;
        }, key, false);
    }
    
}

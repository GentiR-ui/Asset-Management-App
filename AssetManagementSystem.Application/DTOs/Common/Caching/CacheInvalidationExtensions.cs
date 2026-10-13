using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Application.Common.Constants;
using AssetManagementSystem.Domain.Interfaces;
namespace AssetManagementSystem.Application.Common.Caching;
public static class CacheInvalidationExtensions
{
    // 1. Ky overload përdoret kur nuk i jep departmentId (p.sh. fshin vetëm globalen)
    public static Task InvalidateDashboardAsync(
        this ICacheService cache,
        CancellationToken cancellationToken = default)
    {
        return cache.InvalidateDashboardAsync(null, cancellationToken);
    }
    // 2. Ky overload përdoret kur i jep departmentId dhe cancellationToken
    public static async Task InvalidateDashboardAsync(
        this ICacheService cache,
        Guid? departmentId,
        CancellationToken cancellationToken = default)
    {
        foreach (var key in CacheKeys.AllDashboardKeys)
        {
            // Fshihet kasha globale
            await cache.RemoveAsync($"{key}:global", cancellationToken);
            // Nese u specifikua nje departament, fshihet edhe kasha e tij
            if (departmentId.HasValue)
            {
                await cache.RemoveAsync($"{key}:{departmentId.Value}", cancellationToken);
            }
        }
    }
    /*public static async Task InvalidateDashboardAsync(
        this ICacheService cache, Guid? departmentId = null, CancellationToken cancellationToken = default)
    {
        foreach (var key in CacheKeys.AllDashboardKeys)
        {
            await cache.RemoveAsync($"{key}:global", cancellationToken);
            // 2. Nese u prek nje departament specifik, fshihet edhe kasha e atij departamenti
            if (departmentId.HasValue)
            {
                await cache.RemoveAsync($"{key}:{departmentId.Value}", cancellationToken);
            }
        }
    }*/
}

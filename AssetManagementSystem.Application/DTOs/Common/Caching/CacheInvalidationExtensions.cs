using AssetManagementSystem.Application.Common.Constants;
using AssetManagementSystem.Domain.Interfaces;

namespace AssetManagementSystem.Application.Common.Caching;

public static class CacheInvalidationExtensions
{

    public static async Task InvalidateDashboardAsync(
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
    }
}

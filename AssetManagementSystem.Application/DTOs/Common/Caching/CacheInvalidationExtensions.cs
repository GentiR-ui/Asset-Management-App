using AssetManagementSystem.Application.Common.Constants;
using AssetManagementSystem.Domain.Interfaces;

namespace AssetManagementSystem.Application.Common.Caching;

public static class CacheInvalidationExtensions
{

    public static async Task InvalidateDashboardAsync(
        this ICacheService cache, CancellationToken cancellationToken = default)
    {
        foreach (var key in CacheKeys.AllDashboardKeys)
        {
            await cache.RemoveAsync(key, cancellationToken);
        }
    }
}

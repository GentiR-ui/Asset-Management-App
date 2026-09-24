using AssetManagementSystem.Application.Common.Mappings;
using AssetManagementSystem.Application.Common.Constants;
using AssetManagementSystem.Application.DTOs.Dashboard;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Interfaces;
using AssetManagementSystem.Domain.ReadModels;

namespace AssetManagementSystem.Application.Services;


public sealed class DashboardService : IDashboardService
{
    
    private static readonly int[] AgeBreakYears = [1, 3, 5];

    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

    

    private readonly IDashboardRepository _dashboardRepository;
    private readonly ICacheService _cacheService;

    public DashboardService(IDashboardRepository dashboardRepository, ICacheService cacheService)
    {
        _dashboardRepository = dashboardRepository;
        _cacheService = cacheService;
    }

    public async Task<AssetValueSummaryResponse> GetAssetValueSummaryAsync(CancellationToken cancellationToken = default)
    {
        return await GetOrSetAsync(CacheKeys.TotalValue, async () =>
        {
            var summary = await _dashboardRepository.AssetValueSummaryAsync(cancellationToken);

            return summary.ToAssetValueSummaryResponse();
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<DepartmentValueResponse>> GetValueByDepartmentAsync(CancellationToken cancellationToken = default)
    {
        return await GetOrSetAsync(CacheKeys.ByDepartment, async () =>
        {
            var values = await _dashboardRepository.AssetValuesByDepartmentAsync(cancellationToken);

            return values.Select(value => value.ToDepartmentValueResponse()).ToList();
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<StatusValueResponse>> GetAssetsByStatusAsync(CancellationToken cancellationToken = default)
    {
        return await GetOrSetAsync(CacheKeys.ByStatus, async () =>
        {
            var values = await _dashboardRepository.AssetValuesByStatusAsync(cancellationToken);

            return values.Select(value => value.ToStatusValueResponse()).ToList();
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<CategoryValueResponse>> GetAssetsByCategoryAsync(CancellationToken cancellationToken = default)
    {
        return await GetOrSetAsync(CacheKeys.ByCategory, async () =>
        {
            var values = await _dashboardRepository.AssetValuesByCategoryAsync(cancellationToken);

            return values.Select(value => value.ToCategoryValueResponse()).ToList();
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<AgeValueResponse>> GetAssetAgeDistributionAsync(CancellationToken cancellationToken = default)
    {
        return await GetOrSetAsync(CacheKeys.Age, async () =>
        {
            var now = DateTime.UtcNow;

            var cutoffs = new AssetAgeCutoffs(
                now.AddYears(-AgeBreakYears[0]),
                now.AddYears(-AgeBreakYears[1]),
                now.AddYears(-AgeBreakYears[2]));

            var buckets = await _dashboardRepository.AssetValuesByAgeAsync(cutoffs, cancellationToken);

            return buckets.Select(bucket => bucket.ToAgeValueResponse(AgeRangeLabel(bucket.BucketIndex))).ToList();
        }, cancellationToken);
    }

   
    private async Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> loadFromSource,
        CancellationToken cancellationToken) where T : class
    {
        var cached = await _cacheService.GetAsync<T>(key, cancellationToken);

        if (cached is not null)
        {
            return cached;
        }

        var value = await loadFromSource();

        await _cacheService.SetAsync(key, value, CacheTtl, cancellationToken);

        return value;
    }

    /// <summary>0 -> "0-1", 1 -> "1-3", 2 -> "3-5", 3 -> "5+".</summary>
    private static string AgeRangeLabel(int bucketIndex) => bucketIndex switch
    {
        0 => $"0-{AgeBreakYears[0]}",
        1 => $"{AgeBreakYears[0]}-{AgeBreakYears[1]}",
        2 => $"{AgeBreakYears[1]}-{AgeBreakYears[2]}",
        _ => $"{AgeBreakYears[2]}+"
    };
}

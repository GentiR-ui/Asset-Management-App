using AssetManagementSystem.Application.Common.Mappings;
using AssetManagementSystem.Application.DTOs.Dashboard;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Interfaces;
using AssetManagementSystem.Domain.ReadModels;

namespace AssetManagementSystem.Application.Services;

/// <summary>
/// Raportet nuk kane rregulla biznesi qe mund te deshtojne, prandaj asnje metode
/// ketu nuk kthen ErrorOr. Nje liste bosh eshte pergjigje e vlefshme.
/// </summary>
public sealed class DashboardService : IDashboardService
{
    /// <summary>
    /// Kufijte e grupimit sipas moshes, ne vite. Ndryshoji ketu nese ndryshon
    /// cikli i rifreskimit te harduerit: etiketat ndertohen prej tyre.
    /// </summary>
    private static readonly int[] AgeBreakYears = [1, 3, 5];

    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<AssetValueSummaryResponse> GetAssetValueSummaryAsync(CancellationToken cancellationToken = default)
    {
        var summary = await _dashboardRepository.AssetValueSummaryAsync(cancellationToken);

        return summary.ToAssetValueSummaryResponse();
    }

    public async Task<IReadOnlyList<DepartmentValueResponse>> GetValueByDepartmentAsync(CancellationToken cancellationToken = default)
    {
        var values = await _dashboardRepository.AssetValuesByDepartmentAsync(cancellationToken);

        return values.Select(value => value.ToDepartmentValueResponse()).ToList();
    }

    public async Task<IReadOnlyList<StatusValueResponse>> GetAssetsByStatusAsync(CancellationToken cancellationToken = default)
    {
        var values = await _dashboardRepository.AssetValuesByStatusAsync(cancellationToken);

        return values.Select(value => value.ToStatusValueResponse()).ToList();
    }

    public async Task<IReadOnlyList<CategoryValueResponse>> GetAssetsByCategoryAsync(CancellationToken cancellationToken = default)
    {
        var values = await _dashboardRepository.AssetValuesByCategoryAsync(cancellationToken);

        return values.Select(value => value.ToCategoryValueResponse()).ToList();
    }

    public async Task<IReadOnlyList<AgeValueResponse>> GetAssetAgeDistributionAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var cutoffs = new AssetAgeCutoffs(
            now.AddYears(-AgeBreakYears[0]),
            now.AddYears(-AgeBreakYears[1]),
            now.AddYears(-AgeBreakYears[2]));

        var buckets = await _dashboardRepository.AssetValuesByAgeAsync(cutoffs, cancellationToken);

        return buckets.Select(bucket => bucket.ToAgeValueResponse(AgeRangeLabel(bucket.BucketIndex))).ToList();
    }

    
    private static string AgeRangeLabel(int bucketIndex) => bucketIndex switch
    {
        0 => $"0-{AgeBreakYears[0]}",
        1 => $"{AgeBreakYears[0]}-{AgeBreakYears[1]}",
        2 => $"{AgeBreakYears[1]}-{AgeBreakYears[2]}",
        _ => $"{AgeBreakYears[2]}+"
    };
}

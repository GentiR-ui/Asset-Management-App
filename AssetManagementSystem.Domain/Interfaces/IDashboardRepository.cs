using AssetManagementSystem.Domain.ReadModels;

namespace AssetManagementSystem.Domain.Interfaces;

public interface IDashboardRepository
{
    Task<AssetValueSummary> AssetValueSummaryAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DepartmentAssetValue>> AssetValuesByDepartmentAsync(CancellationToken cancellationToken = default);

    /// <summary>I njejti rezultat si AssetValuesByDepartmentAsync, por permes stored procedure.</summary>
    Task<IReadOnlyList<DepartmentAssetValue>> AssetValuesByDepartmentSpAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StatusAssetValue>> AssetValuesByStatusAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CategoryAssetValue>> AssetValuesByCategoryAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AgeBucketCount>> AssetValuesByAgeAsync(AssetAgeCutoffs cutoffs, CancellationToken cancellationToken = default);
}

using AssetManagementSystem.Application.DTOs.Dashboard;

namespace AssetManagementSystem.Application.Interfaces;

public interface IDashboardService
{
    Task<AssetValueSummaryResponse> GetAssetValueSummaryAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DepartmentValueResponse>> GetValueByDepartmentAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StatusValueResponse>> GetAssetsByStatusAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CategoryValueResponse>> GetAssetsByCategoryAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AgeValueResponse>> GetAssetAgeDistributionAsync(CancellationToken cancellationToken = default);
}

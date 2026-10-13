using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.ReadModels;
namespace AssetManagementSystem.Domain.Interfaces;
public interface IAssetRepository
{
    Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Kthen read model, jo entitet: ky rezultat shkon drejt ne pergjigje dhe nuk
    /// modifikohet kurre. Per shkrim perdor GetByIdAsync, qe kthen entitet te gjurmuar.
    /// </summary>
    Task<AssetListItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<AssetListItem>> GetPagedAsync(AssetFilter filter, CancellationToken cancellationToken = default);
    Task<bool> AssetTagExistsAsync(string assetTag, CancellationToken cancellationToken = default);
    Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Asset asset, CancellationToken cancellationToken = default);
    Task UpdateAsync(Asset asset, CancellationToken cancellationToken = default);
    Task RemoveAsync(Asset asset, CancellationToken cancellationToken = default);
    Task<bool> HasAssetsAssignedToEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task UnassignAssetsByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
    /// <summary>Read model: email-i perdor kater fusha, pra query-ja kerkon kater kolona.</summary>
    Task<IReadOnlyList<ExpiringWarrantyItem>> GetExpiringWarrantiesAsync(int daysThreshold, CancellationToken cancellationToken = default);
}

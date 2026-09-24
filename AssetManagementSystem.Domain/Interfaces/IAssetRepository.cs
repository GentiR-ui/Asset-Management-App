using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.ReadModels;

namespace AssetManagementSystem.Domain.Interfaces;

public interface IAssetRepository
{
    Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Asset?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<Asset>> GetPagedAsync(AssetFilter filter, CancellationToken cancellationToken = default);

    Task<bool> AssetTagExistsAsync(string assetTag, CancellationToken cancellationToken = default);

    Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken = default);

    Task AddAsync(Asset asset, CancellationToken cancellationToken = default);

    Task UpdateAsync(Asset asset, CancellationToken cancellationToken = default);

    Task RemoveAsync(Asset asset, CancellationToken cancellationToken = default);

    Task<bool> HasAssetsAssignedToEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);
}

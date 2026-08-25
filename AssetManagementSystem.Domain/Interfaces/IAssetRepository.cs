using AssetManagementSystem.Domain.Entities;

namespace AssetManagementSystem.Domain.Interfaces;

public interface IAssetRepository
{
    Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> AssetTagExistsAsync(string assetTag, CancellationToken cancellationToken = default);

    Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken = default);

    Task AddAsync(Asset asset, CancellationToken cancellationToken = default);

    void Update(Asset asset);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task RemoveAsync(Asset asset, CancellationToken cancellationToken = default);
}

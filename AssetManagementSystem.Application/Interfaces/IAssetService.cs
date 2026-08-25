using AssetManagementSystem.Application.DTOs.Assets;
using ErrorOr;

namespace AssetManagementSystem.Application.Interfaces;

public interface IAssetService
{
    Task<ErrorOr<AssetResponse>> CreateAsync(CreateAssetRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AssetResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ErrorOr<AssetResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ErrorOr<AssetResponse>> UpdateAsync(Guid id, UpdateAssetRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

}

using AssetManagementSystem.Application.DTOs.Assets;
using AssetManagementSystem.Application.DTOs.Common;
using AssetManagementSystem.Domain.Interfaces;
using ErrorOr;
namespace AssetManagementSystem.Application.Interfaces;
public interface IAssetService
{
    Task<ErrorOr<AssetResponse>> CreateAsync(CreateAssetRequest request, CancellationToken cancellationToken = default);
    Task<PagedResponse<AssetResponse>> GetPagedAsync(AssetQueryRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<AssetResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ErrorOr<AssetResponse>> UpdateAsync(Guid id, UpdateAssetRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> AssignAssetToEmployeeAsync(Guid assetId, AssignAssetRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> UnassignAssetFromEmployeeAsync(Guid assetId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> TransferAssetToDepartmentAsync(Guid assetId, Guid newDepartmentId, CancellationToken cancellationToken = default);
    /// <summary>
    /// Liron te gjitha asetet e nje departamenti nga punonjesit qe i mbajne.
    /// Emri i vjeter ishte "Reset", qe linte te kuptohej se asetet hiqeshin nga departamenti.
    /// </summary>
    Task<ErrorOr<Success>> ReleaseAssetsFromEmployeesAsync(Guid departmentId, CancellationToken cancellationToken = default);
}

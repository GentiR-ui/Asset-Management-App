using AssetManagementSystem.Application.DTOs.Common;
using AssetManagementSystem.Application.DTOs.Department;
using AssetManagementSystem.Domain.Interfaces;
using ErrorOr;

namespace AssetManagementSystem.Application.Interfaces;

public interface IDepartmentService
{
    Task<ErrorOr<DepartmentResponse>> CreateDepartmentAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<DepartmentResponse>> UpdateDepartmentAsync(Guid departmentId, UpdateDepartmentRequest request, CancellationToken cancellationToken = default, ICacheService _cacheService = default!);
    Task<ErrorOr<Success>> DeleteDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<ErrorOr<DepartmentResponse>> GetDepartmentByIdAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<PagedResponse<DepartmentResponse>> GetDepartmentsAsync(PageQueryRequest request, CancellationToken cancellationToken = default);
}
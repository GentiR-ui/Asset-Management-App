using AssetManagementSystem.Application.DTOs.Common;
using AssetManagementSystem.Application.DTOs.Employees;
using AssetManagementSystem.Domain.Interfaces;
using ErrorOr;

namespace AssetManagementSystem.Application.Interfaces;

public interface IEmployeeService
{
    Task<ErrorOr<EmployeeResponse>> UpdateEmployeeAsync(Guid employeeId, UpdateEmployeeRequest request, CancellationToken cancellationToken = default, ICacheService _cacheService = default!);
    Task<ErrorOr<EmployeeResponse>> GetEmployeeByIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<PagedResponse<EmployeeResponse>> GetEmployeesAsync(PageQueryRequest request, CancellationToken cancellationToken = default);
}

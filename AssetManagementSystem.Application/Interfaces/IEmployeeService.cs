using AssetManagementSystem.Application.DTOs.Employees;
using ErrorOr;

namespace AssetManagementSystem.Application.Interfaces;

public interface IEmployeeService
{
    Task<ErrorOr<EmployeeResponse>> CreateEmployeeAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<EmployeeResponse>> UpdateEmployeeAsync(Guid employeeId, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> DeleteEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<ErrorOr<EmployeeResponse>> GetEmployeeByIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmployeeResponse>> GetAllEmployeesAsync(CancellationToken cancellationToken = default);
}

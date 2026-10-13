using AssetManagementSystem.Application.DTOs.Common;
using AssetManagementSystem.Application.DTOs.Employees;
using AssetManagementSystem.Domain.Interfaces;
using ErrorOr;
namespace AssetManagementSystem.Application.Interfaces;
public interface IEmployeeService
{
    /// <summary>Lejohet edhe per IT-Manager, brenda departamentit te vet.</summary>
    Task<ErrorOr<EmployeeResponse>> UpdateEmployeeAsync(Guid employeeId, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);

    /// <summary>Levizje mes departamenteve. Vetem Admin.</summary>
    Task<ErrorOr<Success>> TransferEmployeeAsync(Guid employeeId, TransferEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<EmployeeResponse>> GetEmployeeByIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<PagedResponse<EmployeeResponse>> GetEmployeesAsync(PageQueryRequest request, CancellationToken cancellationToken = default);
}

using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.ReadModels;

namespace AssetManagementSystem.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<Employee>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task AddAsync(Employee employee, CancellationToken cancellationToken = default);

    Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default);

    Task RemoveAsync(Employee employee, CancellationToken cancellationToken = default);

    Task<bool> EmployeeCodeExistsAsync(string employeeCode, Guid? excludeId, CancellationToken cancellationToken = default);

    Task<bool> HasEmployeesInDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
}

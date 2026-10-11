using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.ReadModels;

namespace AssetManagementSystem.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Read model, jo entitet: shkon drejt ne pergjigje. Per shkrim: GetByIdAsync.</summary>
    Task<EmployeeListItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>departmentId null = pa filter (Admin). Me vlere = vetem ai departament.</summary>
    Task<PagedResult<EmployeeListItem>> GetPagedAsync(int page, int pageSize, Guid? departmentId, CancellationToken cancellationToken = default);

    Task AddAsync(Employee employee, CancellationToken cancellationToken = default);

    Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default);

    Task RemoveAsync(Employee employee, CancellationToken cancellationToken = default);

    Task<bool> EmployeeCodeExistsAsync(string employeeCode, Guid? excludeId, CancellationToken cancellationToken = default);

    Task<bool> HasEmployeesInDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
}

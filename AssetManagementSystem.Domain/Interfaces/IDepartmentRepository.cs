using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.ReadModels;

namespace AssetManagementSystem.Domain.Interfaces;


public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<Department>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task AddAsync(Department department, CancellationToken cancellationToken = default);

    Task UpdateAsync(Department department, CancellationToken cancellationToken = default);

    Task RemoveAsync(Department department, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, Guid? excludeId,CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, Guid? excludeId,CancellationToken cancellationToken = default);

}
using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Interfaces;
using AssetManagementSystem.Domain.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementSystem.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly ApplicationDbContext _context;

    public DepartmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Department>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Departments.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(department => department.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Department>(items, totalCount);
    }

    public async Task AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Department department, CancellationToken cancellationToken = default)
    {
        _context.Departments.Update(department);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Department department, CancellationToken cancellationToken = default)
    {
        _context.Departments.Remove(department);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId, CancellationToken cancellationToken = default)
    {
        return await _context.Departments.AnyAsync(d => d.Name == name && (excludeId == null || d.Id != excludeId), cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code,Guid? excludeId, CancellationToken cancellationToken = default)
    {
        return await _context.Departments.AnyAsync(d => d.Code == code && (excludeId == null || d.Id != excludeId), cancellationToken);
    }
}
using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Interfaces;
using AssetManagementSystem.Domain.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementSystem.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    
    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    
    public async Task<Employee?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(employee => employee.Department)
            .Include(employee => employee.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    // Pa AsNoTracking: ky entitet do te fshihet, prandaj duhet i gjurmuar.
    public async Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);
    }

    public async Task<PagedResult<Employee>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Employees
            .Include(employee => employee.Department)
            .Include(employee => employee.User)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(employee => employee.EmployeeCode)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Employee>(items, totalCount);
    }

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> EmployeeCodeExistsAsync(string employeeCode, Guid? excludeId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AnyAsync(e => e.EmployeeCode == employeeCode && (excludeId == null || e.Id != excludeId), cancellationToken);
    }

    public async Task<bool> HasEmployeesInDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AnyAsync(e => e.DepartmentId == departmentId, cancellationToken);
    }
}

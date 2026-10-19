using System.Linq.Expressions;
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
    /// <summary>
    /// Projeksioni ne nje vend te vetem, i perdorur nga GetByIdWithDetailsAsync dhe GetPagedAsync.
    /// Brenda Select-it, employee.User.FirstName eshte udhezim per SQL (JOIN), nuk dereferencim.
    /// </summary>
    private static readonly Expression<Func<Employee, EmployeeListItem>> ToListItem =
        employee => new EmployeeListItem(
            employee.Id,
            employee.EmployeeCode,
            employee.User.FirstName,
            employee.User.LastName,
            employee.User.Email ?? string.Empty,
            employee.UserId,
            employee.CreatedAt,
            // Të dhënat e Departamentit
            employee.DepartmentId,
            employee.Department != null ? employee.Department.Name : null,
            employee.Department != null ? employee.Department.Description : null,
            employee.Department != null ? employee.Department.Code : null
        );
    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    // Projeksion: pa Include, pa AsNoTracking. Rezultati nuk eshte entitet.
    public async Task<EmployeeListItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Where(employee => employee.Id == id)
            .Select(ToListItem)
            .FirstOrDefaultAsync(cancellationToken);
    }
    // Pa AsNoTracking: ky entitet do te fshihet, prandaj duhet i gjurmuar.
    public async Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);
    }
    public async Task<PagedResult<EmployeeListItem>> GetPagedAsync(int page, int pageSize, Guid? departmentId, CancellationToken cancellationToken = default)
    {
        var query = _context.Employees.AsQueryable();
        if (departmentId is not null)
        {
            query = query.Where(employee => employee.DepartmentId == departmentId);
        }
        var totalCount = await query.CountAsync(cancellationToken);
        // Select vjen PAS Skip/Take: projeksioni zbatohet vetem mbi rreshtat e faqes.
        var items = await query
            .OrderBy(employee => employee.EmployeeCode)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToListItem)
            .ToListAsync(cancellationToken);
        return new PagedResult<EmployeeListItem>(items, totalCount);
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

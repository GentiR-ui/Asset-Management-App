using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Interfaces;
using AssetManagementSystem.Domain.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementSystem.Infrastructure.Repositories;

public sealed class AssetRepository : IAssetRepository
{
    private readonly ApplicationDbContext _context;

    public AssetRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Assets.FirstOrDefaultAsync(asset => asset.Id == id, cancellationToken);

    
    // Me Include dhe pa gjurmim: vetem per lexim.
    public async Task<Asset?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Assets
            .Include(asset => asset.AssignedToEmployee!).ThenInclude(employee => employee.User)
            .Include(asset => asset.AssignedToEmployee!).ThenInclude(employee => employee.Department)
            .AsNoTracking()
            .FirstOrDefaultAsync(asset => asset.Id == id, cancellationToken);

    public async Task<PagedResult<Asset>> GetPagedAsync(AssetFilter filter, CancellationToken cancellationToken = default)
    {
        // Asgje nuk ekzekutohet ketu: cdo Where vetem e ndertohet mbi IQueryable.
        var query = _context.Assets
            .Include(asset => asset.AssignedToEmployee!).ThenInclude(employee => employee.User)
            .Include(asset => asset.AssignedToEmployee!).ThenInclude(employee => employee.Department)
            .AsNoTracking();

        if (filter.Category is not null)
        {
            query = query.Where(asset => asset.Category == filter.Category);
        }

        if (filter.Status is not null)
        {
            query = query.Where(asset => asset.Status == filter.Status);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();

            query = query.Where(asset => asset.Name.Contains(term)
                                      || asset.AssetTag.Contains(term)
                                      || asset.SerialNumber.Contains(term));
        }

        // Numerimi behet mbi query-n e filtruar, PARA Skip: totali eshte
        // "sa rezultate ka ky filter", jo sa ka tabela.
        var totalCount = await query.CountAsync(cancellationToken);

        // OrderBy eshte i detyrueshem: pa te, SQL Server nuk garanton rend
        // dhe faqja 2 mund te perserise rreshta te faqes 1, pa asnje gabim.
        var items = await query
            .OrderBy(asset => asset.AssetTag)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Asset>(items, totalCount);
    }

    public async Task<bool> AssetTagExistsAsync(string assetTag, CancellationToken cancellationToken = default) =>
       await _context.Assets.AnyAsync(asset => asset.AssetTag == assetTag, cancellationToken);

    public async Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken = default) =>
        await _context.Assets.AnyAsync(asset => asset.SerialNumber == serialNumber, cancellationToken);

    
    public async Task AddAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        await _context.Assets.AddAsync(asset, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        _context.Assets.Update(asset);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    public async Task RemoveAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasAssetsAssignedToEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .AnyAsync(a => a.AssignedToEmployeeId == employeeId, cancellationToken);
    }    
}

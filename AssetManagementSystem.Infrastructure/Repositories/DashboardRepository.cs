using AssetManagementSystem.Domain.Interfaces;
using AssetManagementSystem.Domain.ReadModels;
using Microsoft.EntityFrameworkCore;


namespace AssetManagementSystem.Infrastructure.Repositories;


public sealed class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AssetValueSummary> AssetValueSummaryAsync(CancellationToken cancellationToken = default)
    {
        
        var summary = await _context.Assets
            .GroupBy(_ => 1)
            .Select(group => new AssetValueSummary(
                group.Count(),
                group.Sum(asset => (decimal?)asset.PurchasePrice) ?? 0m))
            .FirstOrDefaultAsync(cancellationToken);

        
        return summary ?? new AssetValueSummary(0, 0m);
    }

    public async Task<IReadOnlyList<DepartmentAssetValue>> AssetValuesByDepartmentAsync(CancellationToken cancellationToken = default)
    {
        var a = new List<String>();
        var b= a.Count;
        var c = a.Count();
        return await _context.Assets
            .GroupBy(asset => new
            {
                Id = (Guid?)asset.AssignedToEmployee!.DepartmentId,
                Name = asset.AssignedToEmployee!.Department.Name
            })
            .OrderBy(group => group.Key.Name)
            .Select(group => new DepartmentAssetValue(
                group.Key.Id,
                group.Key.Name,
                group.Count(),
                group.Sum(asset => asset.PurchasePrice)))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StatusAssetValue>> AssetValuesByStatusAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .GroupBy(asset => asset.Status)
            .OrderBy(group => group.Key)
            .Select(group => new StatusAssetValue(
                group.Key,
                group.Count(),
                group.Sum(asset => asset.PurchasePrice)))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CategoryAssetValue>> AssetValuesByCategoryAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .GroupBy(asset => asset.Category)
            .OrderBy(group => group.Key)
            .Select(group => new CategoryAssetValue(
                group.Key,
                group.Count(),
                group.Sum(asset => asset.PurchasePrice)))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AgeBucketCount>> AssetValuesByAgeAsync(
        AssetAgeCutoffs cutoffs,
        CancellationToken cancellationToken = default)
    {
       
        return await _context.Assets
            .GroupBy(asset =>
                asset.PurchaseDate > cutoffs.First ? 0 :
                asset.PurchaseDate > cutoffs.Second ? 1 :
                asset.PurchaseDate > cutoffs.Third ? 2 : 3)
            .OrderBy(group => group.Key)
            .Select(group => new AgeBucketCount(
                group.Key,
                group.Count(),
                group.Sum(asset => asset.PurchasePrice)))
            .ToListAsync(cancellationToken);
    }
}

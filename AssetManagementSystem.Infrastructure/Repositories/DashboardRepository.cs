using AssetManagementSystem.Domain.Interfaces;
using AssetManagementSystem.Domain.ReadModels;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Dapper;
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
    public async Task<IReadOnlyList<DepartmentAssetValue>> AssetValuesByDepartmentAsync(
        Guid? departmentId,
        CancellationToken cancellationToken = default)
    {
        // Lidhja e EF-it, jo nje e dyte: i njejti connection string dhe i njejti pool.
        var connection = _context.Database.GetDbConnection();
        // @DepartmentId mban scope-in. Rregulli "cili departament i takon kujt" jeton
        // ne AssetScopeResolver; ketu kalohet vetem PERGJIGJA e tij. Nje burim i vertetes,
        // por pa rrjeten e sigurise se filtrit global — harresa ketu nuk ka asnje alarm.
        var command = new CommandDefinition(
            "dbo.GetAssetValueByDepartment",
            parameters: new { DepartmentId = departmentId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
        var rows = await connection.QueryAsync<DepartmentAssetValue>(command);
        return rows.ToList();
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

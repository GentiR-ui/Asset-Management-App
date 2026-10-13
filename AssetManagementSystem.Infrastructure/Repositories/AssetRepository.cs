using System.Security.Claims;
using System.Linq.Expressions;
using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Enums;
using AssetManagementSystem.Domain.Interfaces;
using AssetManagementSystem.Domain.ReadModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace AssetManagementSystem.Infrastructure.Repositories;
public sealed class AssetRepository : IAssetRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AssetRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    /// <summary>
    /// Projeksioni, ne nje vend te vetem: e perdorin edhe GetByIdWithDetailsAsync edhe
    /// GetPagedAsync, pra nje fushe e re shtohet vetem njehere.
    ///
    /// Brenda ketij Select, "asset.Department.Name" NUK eshte dereferencim objekti — eshte
    /// udhezim per SQL, dhe EF-i e perkthen ne LEFT JOIN. Per kete navigimi nuk mund te
    /// jete null ne heshtje, si ndodhte kur Include-i mungonte.
    /// </summary>
    private static readonly Expression<Func<Asset, AssetListItem>> ToListItem =
        asset => new AssetListItem(
            asset.Id,
            asset.AssetTag,
            asset.Name,
            asset.Category,
            asset.SerialNumber,
            asset.Status,
            asset.PurchaseDate,
            asset.PurchasePrice,
            asset.WarrantyExpiryDate,
            asset.Notes,
            asset.CreatedAt,
            asset.DepartmentId,
            asset.Department.Name,
            asset.AssignedToEmployeeId,
            asset.AssignedToEmployee == null
                ? null
                : asset.AssignedToEmployee.User.FirstName + " " + asset.AssignedToEmployee.User.LastName);
    public async Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Assets.FirstOrDefaultAsync(asset => asset.Id == id, cancellationToken);
    // Projeksion: pa Include, pa AsNoTracking. Rezultati nuk eshte entitet.
    public async Task<AssetListItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Assets
            .Where(asset => asset.Id == id)
            .Select(ToListItem)
            .FirstOrDefaultAsync(cancellationToken);
    public async Task<PagedResult<AssetListItem>> GetPagedAsync(AssetFilter filter, CancellationToken cancellationToken = default)
    {
        // Asgje nuk ekzekutohet ketu: cdo Where vetem e ndertohet mbi IQueryable.
        var query = _context.Assets.AsQueryable();
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
        // Select vjen PAS Skip/Take: projeksioni zbatohet vetem mbi rreshtat e faqes,
        // jo mbi te gjithe rezultatin e filtruar.
        var items = await query
            .OrderBy(asset => asset.AssetTag)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(ToListItem)
            .ToListAsync(cancellationToken);
        return new PagedResult<AssetListItem>(items, totalCount);
    }
    public async Task<bool> AssetTagExistsAsync(string assetTag, CancellationToken cancellationToken = default) =>
       await _context.Assets.AnyAsync(asset => asset.AssetTag == assetTag, cancellationToken);
    public async Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken = default) =>
        await _context.Assets.AnyAsync(asset => asset.SerialNumber == serialNumber, cancellationToken);
    public async Task<bool> AssetAtDepartmentExistsAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .AnyAsync(asset => asset.DepartmentId == departmentId, cancellationToken);
    }
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
    public async Task UnassignAssetsByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        // AssignedToEmployeeId != null eshte i domosdoshem, jo optimizim.
        // Pa te, UPDATE-i prek CDO aset te departamentit dhe i kthen te gjithe ne InStock —
        // perfshire ata ne InRepair dhe Retired, qe nuk kane qene kurre te caktuar.
        // U mat: nje reset preku 10,000 rreshta ne vend te 3,332, dhe shkaterroi statuset.
        await _context.Assets
            .Where(a => a.DepartmentId == departmentId && a.AssignedToEmployeeId != null)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(a => a.AssignedToEmployeeId, (Guid?)null)
                .SetProperty(a => a.Status, AssetStatus.InStock),
                cancellationToken);
    }
    public async Task<IReadOnlyList<ExpiringWarrantyItem>> GetExpiringWarrantiesAsync(
        int daysThreshold,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var cutoffDate = now.AddDays(daysThreshold);
        return await _context.Assets
            .Where(asset => asset.WarrantyExpiryDate.HasValue
                         && asset.WarrantyExpiryDate.Value >= now
                         && asset.WarrantyExpiryDate.Value <= cutoffDate)
            .OrderBy(asset => asset.WarrantyExpiryDate)
            .Select(asset => new ExpiringWarrantyItem(
                asset.AssetTag,
                asset.Name,
                asset.WarrantyExpiryDate!.Value,
                asset.Department.Name))
            .ToListAsync(cancellationToken);
    }
}

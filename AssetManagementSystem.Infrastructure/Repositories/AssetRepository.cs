using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Interfaces;
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

    // AsNoTracking: vetem lexim, pa nevoje qe EF t'i ndjeke ndryshimet -> me shpejt, me pak memorie.
    public async Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Assets.AsNoTracking().ToListAsync(cancellationToken);

    public Task<bool> AssetTagExistsAsync(string assetTag, CancellationToken cancellationToken = default) =>
        _context.Assets.AnyAsync(asset => asset.AssetTag == assetTag, cancellationToken);

    public Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken = default) =>
        _context.Assets.AnyAsync(asset => asset.SerialNumber == serialNumber, cancellationToken);

    // AddAsync vetem e shenon per shtim ne memorie — asgje nuk shkon ne DB
    // derisa te thirret SaveChangesAsync.
    public async Task AddAsync(Asset asset, CancellationToken cancellationToken = default) =>
        await _context.Assets.AddAsync(asset, cancellationToken);

    public void Update(Asset asset) => _context.Assets.Update(asset);

    public void Remove(Asset asset) => _context.Assets.Remove(asset);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    public async Task RemoveAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync(cancellationToken);
    }    
}

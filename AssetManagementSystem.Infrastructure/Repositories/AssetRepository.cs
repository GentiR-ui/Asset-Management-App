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

    
    public async Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Assets.AsNoTracking().ToListAsync(cancellationToken);

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
}

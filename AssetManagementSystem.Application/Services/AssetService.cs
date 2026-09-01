using AssetManagementSystem.Application.Common.Mappings;
using AssetManagementSystem.Application.DTOs.Assets;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Errors;
using AssetManagementSystem.Domain.Interfaces;
using AssetManagementSystem.Domain.Enums;
using ErrorOr;

namespace AssetManagementSystem.Application.Services;

public sealed class AssetService : IAssetService
{
    private readonly IAssetRepository _assetRepository;

    public AssetService(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<ErrorOr<AssetResponse>> CreateAsync(
        CreateAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        var assetTag = request.AssetTag.Trim();
        var serialNumber = request.SerialNumber.Trim();

        if (await _assetRepository.AssetTagExistsAsync(assetTag, cancellationToken))
        {
            return AssetErrors.AssetTagAlreadyExists(assetTag);
        }

        if (await _assetRepository.SerialNumberExistsAsync(serialNumber, cancellationToken))
        {
            return AssetErrors.SerialNumberAlreadyExists(serialNumber);
        }

        var asset = new Asset
        {
            AssetTag = assetTag,
            Name = request.Name.Trim(),
            Category = request.Category,
            SerialNumber = serialNumber,
            
            Status = AssetStatus.InStock,
            PurchaseDate = request.PurchaseDate,
            PurchasePrice = request.PurchasePrice,
            WarrantyExpiryDate = request.WarrantyExpiryDate,
            Notes = request.Notes
        };

        await _assetRepository.AddAsync(asset, cancellationToken);

        return asset.ToAssetResponse();
    }

    public async Task<ErrorOr<AssetResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var asset = await _assetRepository.GetByIdAsync(id, cancellationToken);

        return asset is null
            ? AssetErrors.NotFound(id)
            : asset.ToAssetResponse();
    }

    
    public async Task<IReadOnlyList<AssetResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var assets = await _assetRepository.GetAllAsync(cancellationToken);

        return assets.Select(asset => asset.ToAssetResponse()).ToList();
    }

    public async Task<ErrorOr<AssetResponse>> UpdateAsync(
        Guid id,
        UpdateAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        var asset = await _assetRepository.GetByIdAsync(id, cancellationToken);

        if (asset is null)
        {
            return AssetErrors.NotFound(id);
        }

        var assetTag = request.AssetTag.Trim();
        var serialNumber = request.SerialNumber.Trim();

        if (!string.Equals(asset.AssetTag, assetTag, StringComparison.OrdinalIgnoreCase)
            && await _assetRepository.AssetTagExistsAsync(assetTag, cancellationToken))
        {
            return AssetErrors.AssetTagAlreadyExists(assetTag);
        }

        if (!string.Equals(asset.SerialNumber, serialNumber, StringComparison.OrdinalIgnoreCase)
            && await _assetRepository.SerialNumberExistsAsync(serialNumber, cancellationToken))
        {
            return AssetErrors.SerialNumberAlreadyExists(serialNumber);
        }

        asset.AssetTag = assetTag;
        asset.Name = request.Name.Trim();
        asset.SerialNumber = serialNumber;
        asset.Category = request.Category;
        asset.Status = request.Status;
        asset.PurchaseDate = request.PurchaseDate;
        asset.PurchasePrice = request.PurchasePrice;
        asset.WarrantyExpiryDate = request.WarrantyExpiryDate;
        asset.Notes = request.Notes;

        await _assetRepository.UpdateAsync(asset, cancellationToken);

        return asset.ToAssetResponse();
    }

    public async Task<ErrorOr<Success>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var asset = await _assetRepository.GetByIdAsync(id, cancellationToken);

        if (asset is null)
        {
            return AssetErrors.NotFound(id);
        }

        
        await _assetRepository.RemoveAsync(asset, cancellationToken);

        return Result.Success;
    }
}

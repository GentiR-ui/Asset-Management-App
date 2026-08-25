using AssetManagementSystem.Application.DTOs.Assets;
using AssetManagementSystem.Domain.Entities;

namespace AssetManagementSystem.Application.Common.Mappings;

public static class AssetMappings
{
    public static AssetResponse ToAssetResponse(this Asset asset) => new()
    {
        Id = asset.Id,
        AssetTag = asset.AssetTag,
        Name = asset.Name,
        // Enum -> string: klienti merr "Laptop", jo "0".
        Category = asset.Category.ToString(),
        SerialNumber = asset.SerialNumber,
        Status = asset.Status.ToString(),
        PurchaseDate = asset.PurchaseDate,
        PurchasePrice = asset.PurchasePrice,
        WarrantyExpiryDate = asset.WarrantyExpiryDate,
        IsUnderWarranty = asset.WarrantyExpiryDate.HasValue
                          && asset.WarrantyExpiryDate.Value.Date >= DateTime.UtcNow.Date,
        Notes = asset.Notes,
        CreatedAt = asset.CreatedAt
    };
}

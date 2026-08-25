using AssetManagementSystem.Domain.Entities;

namespace AssetManagementSystem.Application.DTOs.Assets;

public sealed record UpdateAssetRequest
{
    public string AssetTag { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string SerialNumber { get; init; } = string.Empty;
    public AssetCategory Category { get; init; }
    public AssetStatus Status { get; init; }
    public DateTime PurchaseDate { get; init; }
    public decimal PurchasePrice { get; init; }
    public DateTime? WarrantyExpiryDate { get; init; }  
    public string? Notes { get; init; }
}

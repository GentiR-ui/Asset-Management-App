using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Enums;

namespace AssetManagementSystem.Application.DTOs.Assets;

public sealed record CreateAssetRequest
{
    public string AssetTag { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public AssetCategory Category { get; init; }
    public string SerialNumber { get; init; } = string.Empty;
    public DateTime PurchaseDate { get; init; }
    public decimal PurchasePrice { get; init; }
    public DateTime? WarrantyExpiryDate { get; init; }
    public string? Notes { get; init; }
}


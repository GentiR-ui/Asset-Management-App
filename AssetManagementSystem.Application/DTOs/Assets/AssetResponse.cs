using AssetManagementSystem.Domain.Entities;

namespace AssetManagementSystem.Application.DTOs.Assets;    

public sealed record AssetResponse
{
    public required Guid Id { get; init; }
    public required string AssetTag { get; init; }
    public required string Name { get; init; }
    public required string Category { get; init; }
    public required string SerialNumber { get; init; }
    public required string Status { get; init; }
    public required DateTime PurchaseDate { get; init; }
    public required decimal PurchasePrice { get; init; }
    public DateTime? WarrantyExpiryDate { get; init; }

    /// <summary>Fushe e llogaritur — klienti s'ka pse ta llogarise vete.</summary>
    public required bool IsUnderWarranty { get; init; }

    public string? Notes { get; init; }
    public required DateTime CreatedAt { get; init; }
}

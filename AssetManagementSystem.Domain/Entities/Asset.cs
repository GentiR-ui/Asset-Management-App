using AssetManagementSystem.Domain.Common;

namespace AssetManagementSystem.Domain.Entities;

public class Asset : BaseEntity
{
    public string AssetTag { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public AssetCategory Category { get; set; }

    public string SerialNumber { get; set; } = string.Empty;

    public AssetStatus Status { get; set; } = AssetStatus.InStock;

    public DateTime PurchaseDate { get; set; }

    public decimal PurchasePrice { get; set; }

    public DateTime? WarrantyExpiryDate { get; set; }

    public string? Notes { get; set; }
}

public enum AssetStatus
{
    InStock = 0,
    Assigned = 1,
    InRepair = 2,
    Retired = 3
}

public enum AssetCategory
{
    Laptop = 0,
    Desktop = 1,
    Monitor = 2,
    Phone = 3,
    Tablet = 4,
    Printer = 5,
    NetworkDevice = 6,
    Furniture = 7,
    Other = 99
}

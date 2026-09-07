using AssetManagementSystem.Domain.Common;
using AssetManagementSystem.Domain.Enums;

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

    public Guid? AssignedToEmployeeId { get; set; }
    public Employee? AssignedToEmployee { get; set; }

}




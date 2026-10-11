using AssetManagementSystem.Application.DTOs.Assets;
using AssetManagementSystem.Domain.ReadModels;

namespace AssetManagementSystem.Application.Common.Mappings;

public static class AssetMappings
{
    /// <summary>
    /// Nga read model-i ne pergjigje. Mapimi ndahet me qellim: SQL-i merr kolonat,
    /// ky metod ben formatimin qe databaza nuk duhet te njohe — enum ne string dhe
    /// fushen e llogaritur IsUnderWarranty.
    /// </summary>
    public static AssetResponse ToAssetResponse(this AssetListItem item) => new()
    {
        Id = item.Id,
        AssetTag = item.AssetTag,
        Name = item.Name,
        Category = item.Category.ToString(),
        SerialNumber = item.SerialNumber,
        Status = item.Status.ToString(),
        PurchaseDate = item.PurchaseDate,
        PurchasePrice = item.PurchasePrice,
        WarrantyExpiryDate = item.WarrantyExpiryDate,
        IsUnderWarranty = item.WarrantyExpiryDate.HasValue
                          && item.WarrantyExpiryDate.Value.Date >= DateTime.UtcNow.Date,
        Notes = item.Notes,
        AssignedToEmployeeId = item.AssignedToEmployeeId,
        AssignedToEmployeeName = item.AssignedToEmployeeName,
        AssignedToDepartmentName = item.DepartmentName,
        DepartmentId = item.DepartmentId,
        CreatedAt = item.CreatedAt
    };
}

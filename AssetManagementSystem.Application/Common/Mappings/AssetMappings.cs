using AssetManagementSystem.Application.DTOs.Assets;
using AssetManagementSystem.Application.DTOs.Department;
using AssetManagementSystem.Application.DTOs.Employees;
using AssetManagementSystem.Domain.ReadModels;
namespace AssetManagementSystem.Application.Common.Mappings;
public static class AssetMappings
{
    /// <summary>
    /// Nga read model-i ne pergjigje. Mapimi ndahet me qellim: SQL-i merr kolonat,
    /// ky metod ben formatimin qe databaza nuk duhet te njohe — enum ne string dhe
    /// fushen e llogaritur IsUnderWarranty.
    /// </summary>
    public static AssetResponse ToAssetResponse(this AssetListItem item)
    {
        return new()
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
            AssignedEmployee = item.EmployeeId.HasValue && item.EmployeeId.Value != Guid.Empty
                ? new EmployeeDto
                {
                    Id = item.EmployeeId.Value,
                    EmployeeCode = item.EmployeeCode ?? string.Empty,
                    FirstName = item.EmployeeFirstName ?? string.Empty,
                    LastName = item.EmployeeLastName ?? string.Empty,
                    Email = item.EmployeeEmail ?? string.Empty
                }
                : null,
            IsUnderWarranty = item.WarrantyExpiryDate.HasValue
                          && item.WarrantyExpiryDate.Value.Date >= DateTime.UtcNow.Date,
            Notes = item.Notes,

            CreatedAt = item.CreatedAt,
            AssignedDepartment = item.DepartmentId.HasValue && item.DepartmentId.Value != Guid.Empty
                ? new DepartmentResponse
                {
                    Id = item.DepartmentId.Value,
                    Name = item.DepartmentName ?? string.Empty,
                    Description = item.DepartmentDescription ?? string.Empty,
                    Code = item.DepartmentCode ?? string.Empty
                }
                : null
        };
    }

}

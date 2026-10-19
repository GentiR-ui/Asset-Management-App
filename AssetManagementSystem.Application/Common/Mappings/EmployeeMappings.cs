using AssetManagementSystem.Application.DTOs.Department;
using AssetManagementSystem.Application.DTOs.Employees;
using AssetManagementSystem.Domain.ReadModels;
namespace AssetManagementSystem.Application.Common.Mappings;
public static class EmployeeMappings
{
    /// <summary>Nga read model-i: asnje navigim, asnje rrezik NullReferenceException.</summary>
    public static EmployeeResponse ToEmployeeResponse(this EmployeeListItem item) => new()
    {
        Id = item.Id,
        EmployeeCode = item.EmployeeCode,
        FirstName = item.FirstName,
        LastName = item.LastName,
        Email = item.Email,
        UserId = item.UserId,
        AssignedDepartment = item.DepartmentId.HasValue && item.DepartmentId.Value != Guid.Empty
                ? new DepartmentResponse
                {
                    Id = item.DepartmentId.Value,
                    Name = item.DepartmentName ?? string.Empty,
                    Description = item.DepartmentDescription ?? string.Empty,
                    Code = item.DepartmentCode ?? string.Empty
                }
                : null,
        CreatedAt = item.CreatedAt
    };
}

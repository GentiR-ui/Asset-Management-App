using AssetManagementSystem.Application.DTOs.Department;
using AssetManagementSystem.Domain.Entities;

namespace AssetManagementSystem.Application.Common.Mappings;

public static class DepartmentMappings
{
    public static DepartmentResponse ToDepartmentResponse(this Department department)
    {
        return new DepartmentResponse
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            Code = department.Code
        };
    }
}
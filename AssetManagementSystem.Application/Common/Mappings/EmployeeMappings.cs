using AssetManagementSystem.Application.DTOs.Employees;
using AssetManagementSystem.Domain.Entities;

namespace AssetManagementSystem.Application.Common.Mappings;

public static class EmployeeMappings
{
    /// <summary>
    /// Kerkon qe User dhe Department te jene te ngarkuar (Include) ose te vendosur me dore.
    /// Pa to hedh NullReferenceException.
    /// </summary>
    public static EmployeeResponse ToEmployeeResponse(this Employee employee)
    {
        return new EmployeeResponse
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            FirstName = employee.User.FirstName,
            LastName = employee.User.LastName,
            Email = employee.User.Email ?? string.Empty,
            UserId = employee.UserId,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department.Name,
            CreatedAt = employee.CreatedAt
        };
    }
}

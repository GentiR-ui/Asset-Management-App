using AssetManagementSystem.Application.DTOs.Department;

namespace AssetManagementSystem.Application.DTOs.Employees;
public sealed record EmployeeResponse
{
    public required Guid Id { get; init; }
    public required string EmployeeCode { get; init; }
    /// <summary>Te tria fushat e meposhtme vijne nga llogaria e lidhur, jo nga tabela Employees.</summary>
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required Guid UserId { get; init; }
    public DepartmentResponse? AssignedDepartment { get; init; }
    public required DateTime CreatedAt { get; init; }
}



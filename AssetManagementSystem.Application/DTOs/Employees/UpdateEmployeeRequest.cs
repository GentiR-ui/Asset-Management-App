namespace AssetManagementSystem.Application.DTOs.Employees;

public sealed record UpdateEmployeeRequest
{
    public string EmployeeCode { get; init; } = string.Empty;

    public Guid DepartmentId { get; init; }
}

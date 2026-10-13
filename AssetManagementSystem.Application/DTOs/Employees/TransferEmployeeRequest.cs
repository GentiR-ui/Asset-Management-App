namespace AssetManagementSystem.Application.DTOs.Employees;

public sealed record TransferEmployeeRequest
{
    public Guid DepartmentId { get; init; }
}

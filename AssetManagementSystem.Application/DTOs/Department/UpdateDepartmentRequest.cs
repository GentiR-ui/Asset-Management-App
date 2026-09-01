namespace AssetManagementSystem.Application.DTOs.Department;

public sealed record UpdateDepartmentRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
}
namespace AssetManagementSystem.Application.DTOs.Employees;

public sealed record CreateEmployeeRequest
{
    public string EmployeeCode { get; init; } = string.Empty;

    /// <summary>Llogaria ekzistuese qe i takon ky punonjes. Emri dhe emaili merren prej saj.</summary>
    public Guid UserId { get; init; }

    public Guid DepartmentId { get; init; }
}

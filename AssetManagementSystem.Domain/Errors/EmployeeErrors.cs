using ErrorOr;

namespace AssetManagementSystem.Domain.Errors;

public static class EmployeeErrors
{
    public static Error NotFound(Guid employeeId) => Error.NotFound(
        code: "Employee.NotFound",
        description: $"Employee with identifier '{employeeId}' was not found.");

    public static Error EmployeeCodeAlreadyExists(string employeeCode) => Error.Conflict(
        code: "Employee.EmployeeCodeAlreadyExists",
        description: $"An employee with code '{employeeCode}' already exists.");

    /// <summary>Mbron FK_Assets_Employees_AssignedToEmployeeId (Restrict).</summary>
    public static Error CannotDeleteWithAssignedAssets(Guid employeeId) => Error.Conflict(
        code: "Employee.CannotDeleteWithAssignedAssets",
        description: $"Employee '{employeeId}' still has assigned assets. Unassign them first.");
}

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

    /// <summary>
    /// Nje punonjes nuk mund te ndryshoje departament sa kohe mban asete: asetet kane
    /// DepartmentId-ne e vet dhe do te mbeteshin te departamenti i vjeter.
    /// </summary>
    public static Error CannotChangeDepartmentWithAssignedAssets(Guid employeeId) => Error.Conflict(
        code: "Employee.CannotChangeDepartmentWithAssignedAssets",
        description: $"Employee '{employeeId}' still has assigned assets. Unassign them before changing department.");

    /// <summary>Mbron FK_Assets_Employees_AssignedToEmployeeId (Restrict).</summary>
    public static Error CannotDeleteWithAssignedAssets(Guid employeeId) => Error.Conflict(
        code: "Employee.CannotDeleteWithAssignedAssets",
        description: $"Employee '{employeeId}' still has assigned assets. Unassign them first.");
}

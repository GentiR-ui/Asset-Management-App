using ErrorOr;
namespace AssetManagementSystem.Domain.Errors;
public static class DepartmentErrors
{
    public static Error NotFound(Guid departmentId) => Error.NotFound(
        code: "Department.NotFound",
        description: $"Department with identifier '{departmentId}' was not found.");
    public static Error DepartmentCodeAlreadyExists(string departmentCode) => Error.Conflict(
        code: "Department.DepartmentCodeAlreadyExists",
        description: $"A department with code '{departmentCode}' already exists.");
    public static Error NameAlreadyExists(string departmentName) => Error.Conflict(
        code: "Department.NameAlreadyExists",
        description: $"A department with name '{departmentName}' already exists.");
    public static Error CodeAlreadyExists(string departmentCode) => Error.Conflict(
        code: "Department.CodeAlreadyExists",
        description: $"A department with code '{departmentCode}' already exists.");        
    /// <summary>Mbron FK_Employees_Departments_DepartmentId (Restrict).</summary>
    public static Error CannotDeleteWithEmployees(Guid departmentId) => Error.Conflict(
        code: "Department.CannotDeleteWithEmployees",
        description: $"Department '{departmentId}' still has employees. Reassign or delete them first.");

    /// <summary>
    /// Mbron FK_Assets_Departments_DepartmentId (Restrict). Pa kete kontroll, databaza
    /// e refuzonte fshirjen me nje gabim te papritur — 500 ne vend te nje 409 te qarte.
    /// </summary>
    public static Error CannotDeleteWithAssets(Guid departmentId) => Error.Conflict(
        code: "Department.CannotDeleteWithAssets",
        description: $"Department '{departmentId}' still has assets. Transfer them to another department first.");
}
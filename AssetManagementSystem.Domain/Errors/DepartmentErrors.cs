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
}
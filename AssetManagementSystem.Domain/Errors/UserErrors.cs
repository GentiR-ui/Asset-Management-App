using ErrorOr;

namespace AssetManagementSystem.Domain.Errors;

public static class UserErrors
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        code: "User.NotFound",
        description: $"User with identifier '{userId}' was not found.");

    /// <summary>
    /// Heqja e rolit Admin nga i fundit që e ka do ta linte sistemin pa asnjë administrator —
    /// dhe askush s'do të mund ta rregullonte nga vetë aplikacioni.
    /// </summary>
    public static Error CannotRemoveLastAdmin => Error.Conflict(
        code: "User.CannotRemoveLastAdmin",
        description: "Cannot remove the Admin role from the last administrator. Assign the Admin role to another user first.");

    public static Error CannotDeleteLastAdmin => Error.Conflict(
        code: "User.CannotDeleteLastAdmin",
        description: "Cannot delete the last administrator. Assign the Admin role to another user first.");
}

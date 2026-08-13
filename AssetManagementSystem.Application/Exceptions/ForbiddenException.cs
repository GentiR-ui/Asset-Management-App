namespace AssetManagementSystem.Application.Exceptions;

/// <summary>
/// Thrown when we <b>know exactly who the caller is</b> and they are still not allowed to do this.
/// <para>Example: an Employee trying to delete an asset, when only Admin may delete.</para>
/// <para>
/// The message is deliberately vague. Saying "only Admins can delete assets" tells an attacker
/// how your permission model is shaped. Log the detail; return the generic text.
/// </para>
/// </summary>
public sealed class ForbiddenException(string message = "You do not have permission to perform this action.") : AppException(message)
{
    public override AppErrorType ErrorType => AppErrorType.Forbidden;
}

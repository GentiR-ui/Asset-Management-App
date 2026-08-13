using ErrorOr;

namespace AssetManagementSystem.Domain.Errors;

public static class IdentityErrors
{
    public static Error EmailAlreadyExists(string email) => Error.Conflict(
        code: "Identity.EmailAlreadyExists",
        description: $"A user with the email '{email}' already exists.");

    public static Error InvalidCredentials => Error.Unauthorized(
        code: "Identity.InvalidCredentials",
        description: "Email or password is incorrect.");

    public static Error UserLockedOut => Error.Forbidden(
        code: "Identity.UserLockedOut",
        description: "This account is temporarily locked due to too many failed attempts.");

    // Mbulon gjithçka që s'e trajtojmë veçmas: PasswordTooShort, PasswordRequiresDigit, etj.
    public static Error FromIdentity(string code, string description) => Error.Validation(
        code: $"Identity.{code}",
        description: description);

    public static Error EmailNotConfirmed => Error.Forbidden(
        code: "Identity.EmailNotConfirmed",
        description: "Please confirm your email address before signing in.");

    public static Error InvalidConfirmationToken => Error.Validation(
        code: "Identity.InvalidConfirmationToken",
        description: "The confirmation link is invalid or has expired.");


}

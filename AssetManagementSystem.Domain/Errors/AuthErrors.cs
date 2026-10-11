using ErrorOr;

namespace AssetManagementSystem.Domain.Errors;

public static class AuthErrors
{
    /// <summary>
    /// NJE gabim i vetem per te gjitha arsyet: nuk ekziston, skadoi, u revokua.
    /// Me qellim — mesazhe te ndryshme i tregojne atakuesit nese token-i qe provoi
    /// ka ekzistuar ndonjehere. I njejti arsyetim si IdentityErrors.InvalidCredentials.
    /// </summary>
    public static Error InvalidRefreshToken => Error.Unauthorized(
        code: "Auth.InvalidRefreshToken",
        description: "The refresh token is invalid or has expired. Please sign in again.");
}

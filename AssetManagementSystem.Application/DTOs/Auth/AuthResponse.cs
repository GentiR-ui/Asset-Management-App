namespace AssetManagementSystem.Application.DTOs.Auth;

public sealed record AuthResponse
{
    public required Guid UserId { get; init; }

    public required string Email { get; init; }

    public required string FullName { get; init; }

    public required string Token { get; init; }

    public required DateTime ExpiresAtUtc { get; init; }

    public required IReadOnlyList<string> Roles { get; init; }
}

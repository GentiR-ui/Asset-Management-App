namespace AssetManagementSystem.Application.DTOs.Account;
public sealed record CurrentUserResponse
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required IReadOnlyList<string> Roles { get; init; }
}
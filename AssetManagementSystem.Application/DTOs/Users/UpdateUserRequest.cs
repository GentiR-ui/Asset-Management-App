namespace AssetManagementSystem.Application.DTOs.Users;

public sealed record UpdateUserRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
}
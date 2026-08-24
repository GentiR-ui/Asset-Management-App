namespace AssetManagementSystem.Application.DTOs.Users;

public sealed record UserResponse
{
    public required Guid Id { get; init; } 
    public required string FirstName { get; init; } 
    public required string LastName { get; init; } 
    public required string Email { get; init; } 
    public bool IsEmailConfirmed { get; init; }
    public required IReadOnlyList<string> Roles { get; init; }
}
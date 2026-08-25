namespace AssetManagementSystem.Application.DTOs.Users;

public sealed record UpdateUserRequest
{
    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;
}

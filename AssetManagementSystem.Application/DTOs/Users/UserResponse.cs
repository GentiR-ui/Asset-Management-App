namespace AssetManagementSystem.Application.DTOs;

public sealed class UserResponse
{
    public string Id { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool IsEmailConfirmed { get; set; }
    public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
}
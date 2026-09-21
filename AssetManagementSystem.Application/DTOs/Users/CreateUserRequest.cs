namespace AssetManagementSystem.Application.DTOs.Users;

/// <summary>
/// Vetem admini e dergon kete. Ndryshe nga RegisterRequest, mban Role dhe te dhenat
/// e punonjesit — prandaj jane dy DTO te ndara: endpoint-i publik nuk mund te mbaje rol.
/// </summary>
public sealed record CreateUserRequest
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;

    /// <summary>Te dyja te detyrueshme per Employee dhe IT-Manager, te panevojshme per Admin.</summary>
    public string? EmployeeCode { get; init; }
    public Guid? DepartmentId { get; init; }
}

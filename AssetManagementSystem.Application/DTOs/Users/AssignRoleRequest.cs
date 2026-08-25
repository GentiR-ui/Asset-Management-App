namespace AssetManagementSystem.Application.DTOs.Users;

public sealed record AssignRoleRequest
{
    // Pa `required`: nese mungon, e kap FluentValidation me nje mesazh te kuptueshem,
    // jo deserializuesi i JSON-it me nje mesazh qe permend emrin e tipit tone.
    public string RoleName { get; init; } = string.Empty;
}

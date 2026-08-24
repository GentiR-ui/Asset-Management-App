namespace AssetManagementSystem.Application.DTOs.Users;
public sealed record AssignRoleRequest
{
    public required string RoleName { get; init; }
}
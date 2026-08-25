namespace AssetManagementSystem.Application.DTOs.Account;

public sealed record ChangePasswordRequest
{
    public string CurrentPassword { get; init; } = string.Empty;

    public string NewPassword { get; init; } = string.Empty;
}

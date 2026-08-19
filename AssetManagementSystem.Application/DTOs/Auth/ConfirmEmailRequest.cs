namespace AssetManagementSystem.Application.DTOs.Auth;

public sealed class ConfirmEmailRequest
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public sealed class ResendConfirmationRequest
{
    public string Email { get; set; } = string.Empty;
}
using AssetManagementSystem.Application.DTOs.Auth;
using ErrorOr;

namespace AssetManagementSystem.Application.Interfaces;

public interface IAuthService
{
    Task<ErrorOr<Success>> RegisterAsync(RegisterRequest request);
    Task<ErrorOr<AuthResponse>> LoginAsync(LoginRequest request);

    Task<ErrorOr<Success>> ConfirmEmailAsync(ConfirmEmailRequest request);

    Task<ErrorOr<Success>> ResendConfirmationAsync(ResendConfirmationRequest request);
    Task<ErrorOr<Success>> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<ErrorOr<Success>> ResetPasswordAsync(ResetPasswordRequest request);

    
}
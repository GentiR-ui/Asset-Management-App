using AssetManagementSystem.Application.DTOs.Auth;
using ErrorOr;

namespace AssetManagementSystem.Application.Interfaces;

public interface IAuthService
{
    Task<ErrorOr<Success>> RegisterAsync(RegisterRequest request);
    Task<ErrorOr<AuthResponse>> LoginAsync(LoginRequest request);

    /// <summary>Shkemben refresh token-in me nje cift te re. Token-i i vjeter revokohet.</summary>
    Task<ErrorOr<AuthResponse>> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken = default);

    /// <summary>Revokon te gjithe refresh token-et e userit aktual.</summary>
    Task<ErrorOr<Success>> LogoutAsync(CancellationToken cancellationToken = default);

    Task<ErrorOr<Success>> ConfirmEmailAsync(ConfirmEmailRequest request);

    Task<ErrorOr<Success>> ResendConfirmationAsync(ResendConfirmationRequest request);
    Task<ErrorOr<Success>> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<ErrorOr<Success>> ResetPasswordAsync(ResetPasswordRequest request);

    
}
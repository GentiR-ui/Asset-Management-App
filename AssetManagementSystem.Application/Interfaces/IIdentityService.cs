using AssetManagementSystem.Application.DTOs.Auth;
using ErrorOr;

namespace AssetManagementSystem.Application.Interfaces;

public interface IIdentityService
{
    Task<ErrorOr<Success>> RegisterAsync(RegisterRequest request);
    Task<ErrorOr<AuthResponse>> LoginAsync(LoginRequest request);

    Task<ErrorOr<Success>> ConfirmEmailAsync(ConfirmEmailRequest request);

    
}
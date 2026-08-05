using AssetManagementSystem.Application.DTOs.Auth;

namespace AssetManagementSystem.Application.Interfaces;

public interface IIdentityService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    
}
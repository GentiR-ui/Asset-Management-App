using AssetManagementSystem.Application.DTOs.Auth;
using AssetManagementSystem.Application.Interfaces;

namespace AssetManagementSystem.Domain.Interfaces;

public class IdentityService : IIdentityService
{
    private readonly IIdentityProvider _identityProvider;

    public IdentityService(IIdentityProvider identityProvider)
    {
        _identityProvider = identityProvider;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        throw new NotImplementedException();
    }
}
using AssetManagementSystem.Application.DTOs.Account;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Interfaces;
using ErrorOr;

namespace AssetManagementSystem.Application.Services;



public sealed class AccountService : IAccountService
{
    private readonly IIdentityProvider _identityProvider;
    public AccountService(IIdentityProvider identityProvider)
    {
        _identityProvider = identityProvider; 
    }
    public async Task<ErrorOr<Success>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        
        var user = await _identityProvider.FindByIdAsync(userId);

        if (user is null)
        {
            
            return Error.NotFound("User.NotFound", "User not found.");
        }

        
        return await _identityProvider.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
    }
}
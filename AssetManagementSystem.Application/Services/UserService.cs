using AssetManagementSystem.Application.Common.Mappings;
using AssetManagementSystem.Application.DTOs.Users;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Interfaces;
using ErrorOr;

namespace AssetManagementSystem.Application.Services;

public class UserService : IUserService
{
    private readonly IIdentityProvider _identityProvider;

    public UserService(IIdentityProvider identityProvider)
    {
        _identityProvider = identityProvider;
    }

    public async Task<IReadOnlyList<UserResponse>> GetUsersAsync()
    {
        var users = await _identityProvider.GetUsersAsync();
        var userResponses = new List<UserResponse>();

        foreach (var u in users)
        {
            var roles = await _identityProvider.GetRolesAsync(u);
            userResponses.Add(u.ToUserResponse(roles));
        }

        return userResponses;
    }

    public async Task<ErrorOr<Success>> AssignRoleAsync(Guid userId, AssignRoleRequest request)
    {
        var user = await _identityProvider.FindByIdAsync(userId);
        if (user is null) return Error.NotFound("User.NotFound", "User was not found.");

        return await _identityProvider.AssignRoleAsync(user, request.RoleName);
    }

    public async Task<ErrorOr<Success>> RemoveRoleAsync(Guid userId, string roleName)
    {
        var user = await _identityProvider.FindByIdAsync(userId);
        if (user is null) return Error.NotFound("User.NotFound", "User was not found.");

        return await _identityProvider.RemoveRoleAsync(user, roleName);
    }

    public async Task<ErrorOr<Success>> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        
        var user = await _identityProvider.FindByIdAsync(userId);
        if (user is null)
        {
            return Error.NotFound("User.NotFound", "User was not found.");
        }

        
        return await _identityProvider.UpdateUserAsync(user, request.FirstName, request.LastName, request.Email);
    }

    public async Task<ErrorOr<Success>> DeleteUserAsync(Guid userId)
        {
            var user = await _identityProvider.FindByIdAsync(userId);
            if (user is null)
            {
                return Error.NotFound("User.NotFound", "User was not found.");
            }

            return await _identityProvider.DeleteUserAsync(user);
        }
}
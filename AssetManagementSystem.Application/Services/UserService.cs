using AssetManagementSystem.Application.Common.Mappings;
using AssetManagementSystem.Application.DTOs.Users;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Common;
using AssetManagementSystem.Domain.Errors;
using AssetManagementSystem.Domain.Interfaces;
using ErrorOr;

namespace AssetManagementSystem.Application.Services;

public class UserService : IUserService
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IEmployeeRepository _employeeRepository;

    public UserService(IIdentityProvider identityProvider, IEmployeeRepository employeeRepository)
    {
        _identityProvider = identityProvider;
        _employeeRepository = employeeRepository;
    }

    public async Task<IReadOnlyList<UserResponse>> GetUsersAsync()
    {
        var users = await _identityProvider.GetUsersAsync();
        var userResponses = new List<UserResponse>();

        // TODO: N+1 — nje query per cdo user. Optimizoje me nje JOIN te vetem ne Fazen 2.
        foreach (var user in users)
        {
            var roles = await _identityProvider.GetRolesAsync(user);
            userResponses.Add(user.ToUserResponse(roles));
        }

        return userResponses;
    }

    public async Task<ErrorOr<Success>> AssignRoleAsync(Guid userId, AssignRoleRequest request)
    {
        var user = await _identityProvider.FindByIdAsync(userId);

        if (user is null)
        {
            return UserErrors.NotFound(userId);
        }

        return await _identityProvider.AssignRoleAsync(user, request.RoleName);
    }

    public async Task<ErrorOr<Success>> RemoveRoleAsync(Guid userId, string roleName)
    {
        var user = await _identityProvider.FindByIdAsync(userId);

        if (user is null)
        {
            return UserErrors.NotFound(userId);
        }

        if (await IsLastAdminAsync(user, roleName))
        {
            return UserErrors.CannotRemoveLastAdmin;
        }

        return await _identityProvider.RemoveRoleAsync(user, roleName);
    }

    public async Task<ErrorOr<Success>> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        var user = await _identityProvider.FindByIdAsync(userId);

        if (user is null)
        {
            return UserErrors.NotFound(userId);
        }

        return await _identityProvider.UpdateUserAsync(
            user, request.FirstName, request.LastName);
    }

    public async Task<ErrorOr<Success>> DeleteUserAsync(Guid userId)
    {
        var user = await _identityProvider.FindByIdAsync(userId);

        if (user is null)
        {
            return UserErrors.NotFound(userId);
        }

        if (await IsLastAdminAsync(user, AppRoles.Admin))
        {
            return UserErrors.CannotDeleteLastAdmin;
        }

        // FK_Employees_Users_UserId eshte Restrict: pa kete kontroll, SQL-i e refuzon
        // fshirjen dhe perdoruesi merr 500 ne vend te nje mesazhi te kuptueshem.
        if (await _employeeRepository.IsUserLinkedAsync(userId))
        {
            return UserErrors.CannotDeleteLinkedToEmployee;
        }

        return await _identityProvider.DeleteUserAsync(user);
    }

    /// <summary>
    /// A eshte ky useri i FUNDIT qe e mban rolin Admin?
    /// Nese po, heqja e rolit ose fshirja e tij do ta linte sistemin pa asnje administrator —
    /// dhe askush s'do te mund ta rregullonte nga vete aplikacioni.
    /// </summary>
    private async Task<bool> IsLastAdminAsync(Domain.Entities.User user, string roleName)
    {
        if (!string.Equals(roleName, AppRoles.Admin, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var roles = await _identityProvider.GetRolesAsync(user);

        if (!roles.Contains(AppRoles.Admin))
        {
            return false;
        }

        var adminCount = await _identityProvider.CountUsersInRoleAsync(AppRoles.Admin);

        return adminCount <= 1;
    }
}

using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Errors;
using AssetManagementSystem.Domain.Interfaces;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementSystem.Infrastructure.Identity;

public class IdentityProvider : IIdentityProvider
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public IdentityProvider(
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<ErrorOr<User>> CreateUserAsync(
        string firstName,
        string lastName,
        string email,
        string password)
    {
        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserName = email
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return user;
        }

        return result.Errors
            .Select(identityError => MapIdentityError(identityError, email))
            .DistinctBy(error => error.Code)
            .ToList();
    }

    public async Task<ErrorOr<User>> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        
        if (user is null)
        {
            return IdentityErrors.InvalidCredentials;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            password,
            lockoutOnFailure: true);

        

        if (result.IsLockedOut)
        {
            return IdentityErrors.UserLockedOut;
        }


        if (result.IsNotAllowed)
        {
            return IdentityErrors.EmailNotConfirmed;
        }


        if (!result.Succeeded)
        {
            return IdentityErrors.InvalidCredentials;
        }

        return user;
    }

    public Task<IList<string>> GetRolesAsync(User user) => _userManager.GetRolesAsync(user);

    public Task AddToRoleAsync(User user, string role) => _userManager.AddToRoleAsync(user, role);

    private static Error MapIdentityError(IdentityError identityError, string email) =>
        identityError.Code switch
        {
            "DuplicateEmail" or "DuplicateUserName" => IdentityErrors.EmailAlreadyExists(email),
            _ => IdentityErrors.FromIdentity(identityError.Code, identityError.Description)
        };

    public Task<string> GenerateEmailConfirmationTokenAsync(User user) =>
    _userManager.GenerateEmailConfirmationTokenAsync(user);

    public Task<User?> FindByEmailAsync(string email) => _userManager.FindByEmailAsync(email);

    public async Task<ErrorOr<Success>> ConfirmEmailAsync(User user, string token)
    {
        var result = await _userManager.ConfirmEmailAsync(user, token);

        return result.Succeeded
            ? Result.Success
            : IdentityErrors.InvalidConfirmationToken;
    }
    public Task<string> GeneratePasswordResetTokenAsync(User user) =>
    _userManager.GeneratePasswordResetTokenAsync(user);

    public async Task<ErrorOr<Success>> ResetPasswordAsync(User user, string token, string newPassword)
    {
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (result.Succeeded)
        {
            return Result.Success;
        }

        return result.Errors
            .Select(identityError => identityError.Code is "InvalidToken"
                ? IdentityErrors.InvalidPasswordResetToken
                : IdentityErrors.FromIdentity(identityError.Code, identityError.Description))
            .DistinctBy(error => error.Code)
            .ToList();
    }

    public async Task<ErrorOr<Success>> ChangePasswordAsync(User user, string currentPassword, string newPassword)
    {
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (result.Succeeded)
        {
            return Result.Success;
        }

        return result.Errors
            .Select(identityError => IdentityErrors.FromIdentity(identityError.Code, identityError.Description))
            .DistinctBy(error => error.Code)
            .ToList();
    }

    public async Task<User?> FindByIdAsync(Guid userId) => await _userManager.FindByIdAsync(userId.ToString());

    public async Task<IReadOnlyList<User>> GetUsersAsync() =>
        await _userManager.Users.ToListAsync();


    public async Task<ErrorOr<Success>> AssignRoleAsync(User user, string roleName)
    {
        var result = await _userManager.AddToRoleAsync(user, roleName);

        if (result.Succeeded)
            return Result.Success;

        // "E ka veç atë rol" nuk është gabim formati — është përplasje gjendjeje → 409.
        return result.Errors
            .Select(identityError => identityError.Code is "UserAlreadyInRole"
                ? IdentityErrors.UserAlreadyInRole(identityError.Description)
                : IdentityErrors.FromIdentity(identityError.Code, identityError.Description))
            .DistinctBy(error => error.Code)
            .ToList();
    }

    public async Task<ErrorOr<Success>> RemoveRoleAsync(User user, string roleName)
    {
        var result = await _userManager.RemoveFromRoleAsync(user, roleName);

        if (result.Succeeded)
            return Result.Success;

        return result.Errors
            .Select(identityError => identityError.Code is "UserNotInRole"
                ? IdentityErrors.UserNotInRole(identityError.Description)
                : IdentityErrors.FromIdentity(identityError.Code, identityError.Description))
            .DistinctBy(error => error.Code)
            .ToList();
    }

    public async Task<ErrorOr<Success>> UpdateUserAsync(User user, string firstName, string lastName)
    {
        user.FirstName = firstName;
        user.LastName = lastName;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Result.Success;

        return result.Errors
            .Select(identityError => IdentityErrors.FromIdentity(identityError.Code, identityError.Description))
            .DistinctBy(error => error.Code)
            .ToList();
    }

    public async Task<ErrorOr<Success>> DeleteUserAsync(User user)
    {
        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
            return Result.Success;

        return result.Errors
            .Select(identityError => IdentityErrors.FromIdentity(identityError.Code, identityError.Description))
            .DistinctBy(error => error.Code)
            .ToList();
    }





    public async Task<int> CountUsersInRoleAsync(string roleName)
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
        return usersInRole.Count;
    }
}

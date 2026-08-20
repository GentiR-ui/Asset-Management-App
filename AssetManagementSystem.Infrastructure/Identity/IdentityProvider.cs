using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Errors;
using AssetManagementSystem.Domain.Interfaces;
using ErrorOr;
using Microsoft.AspNetCore.Identity;

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

        // Nëse useri s'ekziston kthejmë TË NJËJTIN gabim si për password gabim.
        // Ndryshe dikush mund t'i provojë email-at një nga një dhe të mësojë cilët ekzistojnë.
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

 



}

using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Interfaces;
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

    public async Task<bool> CreateUserAsync(
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

        return result.Succeeded;
    }

    public async Task<bool> ValidatePasswordAsync(
        string email,
        string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return false;

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            password,
            false);

        return result.Succeeded;
    }
}
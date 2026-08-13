using AssetManagementSystem.Application.DTOs.Auth;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Common;
using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Errors;
using AssetManagementSystem.Domain.Interfaces;
using ErrorOr;

namespace AssetManagementSystem.Application.Services;

public class IdentityService : IIdentityService
{
    private readonly IIdentityProvider _identityProvider;

    

    private readonly IEmailSender _emailSender;

    public IdentityService(IIdentityProvider identityProvider, IEmailSender emailSender)
    {
        _identityProvider = identityProvider;
        _emailSender = emailSender;
    }


    public async Task<ErrorOr<Success>> RegisterAsync(RegisterRequest request)
    {
        var createResult = await _identityProvider.CreateUserAsync(
            request.FirstName, request.LastName, request.Email, request.Password);

        if (createResult.IsError)
        {
            return createResult.Errors;
        }

        var user = createResult.Value;
        await _identityProvider.AddToRoleAsync(user, AppRoles.Employee);


        
        var token = await _identityProvider.GenerateEmailConfirmationTokenAsync(user);

        await _emailSender.SendAsync(
            to: request.Email,
            subject: "Confirm your email",
            body: $"Your confirmation token:\n\n{token}");
        

        return Result.Success;
    }

    public async Task<ErrorOr<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var validateResult = await _identityProvider.ValidateCredentialsAsync(request.Email, request.Password);

        if (validateResult.IsError)
        {
            return validateResult.Errors;
        }

        var user = validateResult.Value;

        return await BuildAuthResponseAsync(user);
    }

    private async Task<AuthResponse> BuildAuthResponseAsync(User user)
    {
        var roles = await _identityProvider.GetRolesAsync(user);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = $"{user.FirstName} {user.LastName}",
            Token = "TODO-jwt-not-implemented-yet",
            ExpiresAtUtc = DateTime.UtcNow.AddHours(1),
            Roles = roles.ToList()
        };
    }

    public async Task<ErrorOr<Success>> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        var user = await _identityProvider.FindByEmailAsync(request.Email);

        // Mos zbulo nëse email-i ekziston — i njëjti gabim si për token të keq.
        if (user is null)
        {
            return IdentityErrors.InvalidConfirmationToken;
        }

        return await _identityProvider.ConfirmEmailAsync(user, request.Token);
    }




}
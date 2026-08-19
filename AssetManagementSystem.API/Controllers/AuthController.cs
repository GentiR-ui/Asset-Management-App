
using AssetManagementSystem.Application.DTOs.Auth;
using AssetManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AssetManagementSystem.API.Controllers;

[Route("api/auth")]
public sealed class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        return result.Match<IActionResult>(
        _ => Success("Operation completed successfully."),
        Problem);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        return result.Match(
        response => Success(response),
        Problem);
    }

    [AllowAnonymous]
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
    {
        var result = await _authService.ConfirmEmailAsync(request);

        return result.Match<IActionResult>(
        _ => Success("Email confirmed successfully. You can now sign in."),
        Problem);
    }

    [AllowAnonymous]
    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmation(ResendConfirmationRequest request)
    {
        var result = await _authService.ResendConfirmationAsync(request);

        return result.Match<IActionResult>(
        _ => Success("If that email is registered and not yet confirmed, a confirmation link has been sent."),
        Problem);
    }

    [HttpGet("me")]
    public IActionResult Me() => Success(new
    {
        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
        Email  = User.FindFirstValue(ClaimTypes.Email),
        Roles  = User.FindAll(ClaimTypes.Role).Select(claim => claim.Value)
    });

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var result = await _authService.ForgotPasswordAsync(request);
        return result.Match<IActionResult>(
        _ => Success("If that email is registered, a password reset link has been sent."),
        Problem);
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        return result.Match<IActionResult>(
        _ => Success("Password has been reset. You can now sign in with your new password."),
        Problem);
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout() => Success("Logged out successfully.");




}

using AssetManagementSystem.Application.DTOs.Auth;
using AssetManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using AssetManagementSystem.API.Constants;

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
    [EnableRateLimiting(RateLimitPolicies.AuthPolicy)]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return HandleResult(result, "Operation completed successfully.");
    }

    [AllowAnonymous]
    [EnableRateLimiting(RateLimitPolicies.AuthPolicy)]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return HandleResult(result);
    }
    [AllowAnonymous]
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
    {
        var result = await _authService.ConfirmEmailAsync(request);
        return HandleResult(result, "Email confirmed successfully. You can now sign in.");
    }
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitPolicies.AuthPolicy)]
    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmation(ResendConfirmationRequest request)
    {
        var result = await _authService.ResendConfirmationAsync(request);
        return HandleResult(result, "If that email is registered and not yet confirmed, a confirmation link has been sent.");
    }
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitPolicies.AuthPolicy)]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var result = await _authService.ForgotPasswordAsync(request);
        return HandleResult(result, "If that email is registered, a password reset link has been sent.");
    }
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        return HandleResult(result, "Password has been reset. You can now sign in with your new password.");
    }
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request, CancellationToken cancellationToken)
    {
        // AllowAnonymous me qellim: /refresh thirret pikerisht kur access token-i ka skaduar.
        // Autorizimi behet nga refresh token-i i trupit, i cili kerkohet ne databaze.
        var result = await _authService.RefreshAsync(request, cancellationToken);
        return HandleResult(result);
    }
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var result = await _authService.LogoutAsync(cancellationToken);
        return HandleResult(result, "Logged out successfully.");
    }
}

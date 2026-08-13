
using AssetManagementSystem.Application.DTOs.Auth;
using AssetManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementSystem.API.Controllers;

[Route("api/auth")]
public sealed class AuthController : ApiControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _identityService.RegisterAsync(request);

        return result.Match<IActionResult>(_ => NoContent(), Problem);                   // ← rasti i gabimit
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _identityService.LoginAsync(request);

        return result.Match(
            response => Ok(response),
            Problem);
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
    {
        var result = await _identityService.ConfirmEmailAsync(request);

        return result.Match<IActionResult>(_ => NoContent(), Problem);
    }


}

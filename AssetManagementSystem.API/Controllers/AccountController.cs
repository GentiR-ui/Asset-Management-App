using AssetManagementSystem.Application.DTOs.Auth;
using AssetManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using AssetManagementSystem.API.Controllers;
using AssetManagementSystem.Application.DTOs.Account;

namespace AssetManagementSystem.API.Controllers;

[Authorize]
[Route("api/account")]
public sealed class AccountController : ApiControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("me")]
    public IActionResult Me() => Success(new CurrentUserResponse
    {
        UserId = CurrentUserId,
        Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
        Roles  = User.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToList()
    });

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        
        var result = await _accountService.ChangePasswordAsync(CurrentUserId, request);

        return result.Match<IActionResult>(
        _ => Success("Password changed successfully."),
        Problem);
    }
}
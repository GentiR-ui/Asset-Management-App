using AssetManagementSystem.Application.DTOs.Users;
using AssetManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementSystem.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/users")]
public sealed class UserController : ApiControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetUsersAsync();
        return Ok(users);
    }

    [HttpPost("{userId:guid}/roles")]
    public async Task<IActionResult> AssignRole(Guid userId, [FromBody] AssignRoleRequest request)
    {
        var result = await _userService.AssignRoleAsync(userId, request);

        return result.Match(
            _ => Success("Role assigned successfully."),
            Problem);
    }

    [HttpDelete("{userId:guid}/roles/{roleName}")]  
    public async Task<IActionResult> RemoveRole(Guid userId, string roleName)
    {
        var result = await _userService.RemoveRoleAsync(userId, roleName);

        return result.Match(
            _ => Success("Role removed successfully."),
            Problem);
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
    {
        var result = await _userService.UpdateUserAsync(userId, request);

        return result.Match(
            _ => Success("User updated successfully."),
            Problem);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var result = await _userService.DeleteUserAsync(userId);

        return result.Match(
            _ => Success("User deleted successfully."),
            Problem);
    }
}
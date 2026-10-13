using AssetManagementSystem.Application.DTOs.Common;
using AssetManagementSystem.Application.DTOs.Users;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AssetManagementSystem.API.Controllers;
[Authorize(Roles = AppRoles.Admin)]
[Route("api/users")]
public sealed class UserController : ApiControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.CreateUserAsync(request, cancellationToken);
        return HandleResult(result, StatusCodes.Status201Created);
    }
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] PageQueryRequest request)
    {
        var users = await _userService.GetUsersAsync(request);
        return Success(users);
    }
    [HttpPost("{userId:guid}/roles")]
    public async Task<IActionResult> AssignRole(Guid userId, [FromBody] AssignRoleRequest request)
    {
        var result = await _userService.AssignRoleAsync(userId, request);
        return HandleResult(result, "Role assigned successfully.");
    }
    [HttpDelete("{userId:guid}/roles/{roleName}")]  
    public async Task<IActionResult> RemoveRole(Guid userId, string roleName)
    {
        var result = await _userService.RemoveRoleAsync(userId, roleName);
        return HandleResult(result, "Role removed successfully.");
    }
    // Emri dhe mbiemri jetojne te User, jo te Employee — pra per t'i ndryshuar te dhenat
    // e nje punonjesi duhet ky endpoint. Hapet per IT-Manager, dhe UserService kontrollon
    // qe useri i prekur te jete punonjes i departamentit te tij.
    //
    // Nje Admin nuk ka rresht te Employees, pra nje menaxher nuk mund ta prekë kurre.
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ItManager}")]
    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
    {
        var result = await _userService.UpdateUserAsync(userId, request);
        return HandleResult(result, "User updated successfully.");
    }
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        var result = await _userService.DeleteUserAsync(userId);
        return HandleResult(result, "User deleted successfully.");
    }
}
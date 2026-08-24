using AssetManagementSystem.Application.DTOs.Users;
using ErrorOr;

namespace AssetManagementSystem.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetUsersAsync();

    Task<ErrorOr<Success>> AssignRoleAsync(Guid userId, AssignRoleRequest request);

    Task<ErrorOr<Success>> RemoveRoleAsync(Guid userId, string roleName);

    Task<ErrorOr<Success>> UpdateUserAsync(Guid userId, UpdateUserRequest request);

    Task<ErrorOr<Success>> DeleteUserAsync(Guid userId);

}

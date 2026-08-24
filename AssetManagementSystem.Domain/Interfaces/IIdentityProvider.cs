using AssetManagementSystem.Domain.Entities;
using ErrorOr;

namespace AssetManagementSystem.Domain.Interfaces;

public interface IIdentityProvider
{
    Task<ErrorOr<User>> CreateUserAsync(
        string firstName,
        string lastName,
        string email,
        string password);

    Task<ErrorOr<User>> ValidateCredentialsAsync(string email, string password);

    Task<IList<string>> GetRolesAsync(User user);

    Task AddToRoleAsync(User user, string role);

    Task<string> GenerateEmailConfirmationTokenAsync(User user);
    Task<ErrorOr<Success>> ConfirmEmailAsync(User user, string token);
    Task<User?> FindByEmailAsync(string email);
    Task<string> GeneratePasswordResetTokenAsync(User user);
    Task<ErrorOr<Success>> ResetPasswordAsync(User user, string token, string newPassword);
    Task<ErrorOr<Success>> ChangePasswordAsync(User user, string currentPassword, string newPassword);
    Task<User?> FindByIdAsync(Guid userId);
    Task<IReadOnlyList<User>> GetUsersAsync();
    Task<ErrorOr<Success>> AssignRoleAsync(User user, string roleName);
    Task<ErrorOr<Success>> RemoveRoleAsync(User user, string roleName);
    Task<ErrorOr<Success>> UpdateUserAsync(User user, string firstName, string lastName, string email);
    Task<ErrorOr<Success>> DeleteUserAsync(User user);
    
    
}

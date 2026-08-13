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

}

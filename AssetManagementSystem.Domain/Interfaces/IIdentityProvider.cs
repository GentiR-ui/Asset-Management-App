namespace AssetManagementSystem.Domain.Interfaces;

public interface IIdentityProvider
{
    Task<bool> CreateUserAsync(
        string firstName,
        string lastName,
        string email,
        string password);

    Task<bool> ValidatePasswordAsync(
        string email,
        string password);
}
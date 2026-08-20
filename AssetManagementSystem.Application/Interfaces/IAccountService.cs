using AssetManagementSystem.Application.DTOs.Account;
using ErrorOr;

namespace AssetManagementSystem.Application.Interfaces;

public interface IAccountService
{
    Task<ErrorOr<Success>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
}
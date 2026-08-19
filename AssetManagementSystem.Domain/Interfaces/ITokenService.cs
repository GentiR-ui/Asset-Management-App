using AssetManagementSystem.Domain.Common;
using AssetManagementSystem.Domain.Entities;

namespace AssetManagementSystem.Domain.Interfaces;

public interface ITokenService
{
    AccessToken GenerateToken(User user, IEnumerable<string> roles);
}


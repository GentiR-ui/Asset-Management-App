using AssetManagementSystem.Application.DTOs.Auth;
using AssetManagementSystem.Domain.Common;
using AssetManagementSystem.Domain.Entities;

namespace AssetManagementSystem.Application.Common.Mappings;

public static class AuthMappings
{
    public static AuthResponse ToAuthResponse(
        this User user,
        AccessToken accessToken,
        IEnumerable<string> roles) => new()
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = $"{user.FirstName} {user.LastName}",
            Token = accessToken.Value,
            ExpiresAtUtc = accessToken.ExpiresAtUtc,
            Roles = roles.ToList()

        };
}

using AssetManagementSystem.Application.DTOs.Auth;
using AssetManagementSystem.Domain.Common;
using AssetManagementSystem.Domain.Entities;

namespace AssetManagementSystem.Application.Common.Mappings;

public static class AuthMappings
{
    public static AuthResponse ToAuthResponse(
        this User user,
        AccessToken accessToken,
        RefreshTokenValue refreshToken,
        IEnumerable<string> roles) => new()
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = $"{user.FirstName} {user.LastName}",
            Token = accessToken.Value,
            ExpiresAtUtc = accessToken.ExpiresAtUtc,
            // Vlera e papërpunuar: kjo është e vetmja herë që klienti e shef.
            RefreshToken = refreshToken.Value,
            RefreshTokenExpiresAtUtc = refreshToken.ExpiresAtUtc,
            Roles = roles.ToList()

        };
}

using AssetManagementSystem.Domain.Common;
using AssetManagementSystem.Domain.Entities;
namespace AssetManagementSystem.Domain.Interfaces;
public interface ITokenService
{
    /// <summary>
    /// Token-i mban vetem identitetin: kush eshte dhe cfare rolesh ka.
    /// Departamenti eshte autorizim qe ndryshon, prandaj lexohet ne cdo kerkese.
    /// </summary>
    AccessToken GenerateToken(User user, IEnumerable<string> roles);
    /// <summary>
    /// Token i rastesishem kriptografikisht, bashke me hash-in e vet. Guid.NewGuid() NUK
    /// perdoret: Guid-i eshte unik, por i parashikueshem — nuk eshte sekret.
    /// </summary>
    RefreshTokenValue GenerateRefreshToken();
    /// <summary>Hash-i i te njejtes forme si te GenerateRefreshToken, per kerkim ne databaze.</summary>
    string HashRefreshToken(string rawToken);
}

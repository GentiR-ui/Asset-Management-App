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
    
}


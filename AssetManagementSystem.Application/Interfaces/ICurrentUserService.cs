namespace AssetManagementSystem.Application.Interfaces;
/// <summary>
/// Vetem ajo qe mban token-i: identiteti. Departamenti lexohet nga AssetScopeResolver.
/// Konsumohet vetem nga Application. Infrastructure e lexon userin permes
/// CurrentUserAccessor, qe mban te njejten logjike claim-esh.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>Guid.Empty kur nuk ka kerkese HTTP (migrim, seed, background service).</summary>
    Guid UserId { get; }
    string Role { get; }
}

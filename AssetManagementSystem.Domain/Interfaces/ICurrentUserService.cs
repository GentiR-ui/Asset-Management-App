namespace AssetManagementSystem.Domain.Interfaces;

/// <summary>
/// Vetem ajo qe mban token-i: identiteti. Departamenti lexohet nga AssetScopeResolver.
///
/// Rri ne Domain sepse e kerkojne edhe ApplicationDbContext edhe interceptori, qe jetojne
/// ne Infrastructure — dhe Infrastructure referencon vetem Domain. I njejti arsyetim
/// si per IScopeContext dhe ICacheService.
///
/// Implementimi duhet te mbetet PA GJENDJE: regjistrohet Singleton, sepse interceptori
/// qe e konsumon eshte Singleton.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>Guid.Empty kur nuk ka kerkese HTTP (migrim, seed, background service).</summary>
    Guid UserId { get; }

    string Role { get; }
}

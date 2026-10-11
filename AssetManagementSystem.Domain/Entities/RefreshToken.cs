using AssetManagementSystem.Domain.Common;

namespace AssetManagementSystem.Domain.Entities;

/// <summary>
/// Kuponi per nje access token te re. Ndryshe nga access token-i, KY ruhet ne databaze —
/// dhe pikerisht per kete mund te revokohet. Logout = revokim i ketij rreshti.
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>
    /// SHA-256 i token-it, jo token-i. Nese databaza rrjedh, hash-i nuk hyn ne pune:
    /// i njejti arsyetim si per fjalekalimet.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public DateTime ExpiresAtUtc { get; set; }

    /// <summary>null = ende i vlefshem. Me vlere = i revokuar, dhe kurre me i pranueshem.</summary>
    public DateTime? RevokedAtUtc { get; set; }

    /// <summary>
    /// Token-i qe e zevendesoi ne rotacion. Lidh zinxhirin e token-eve te nje sesioni,
    /// qe nese nje i revokuar riperdoret, te dihet cili sesion duhet shuar i teri.
    /// </summary>
    public Guid? ReplacedByTokenId { get; set; }

    public bool IsActive(DateTime utcNow) => RevokedAtUtc is null && ExpiresAtUtc > utcNow;
}

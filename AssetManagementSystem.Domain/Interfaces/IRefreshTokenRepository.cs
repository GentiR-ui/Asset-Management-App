using AssetManagementSystem.Domain.Entities;

namespace AssetManagementSystem.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    /// <summary>Kerkimi behet me hash, sepse vetem hash-i ndodhet ne databaze.</summary>
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokon te vjetrin dhe shton te rine ne NJE SaveChanges. Nje metode e vetme sepse
    /// gjysma e rotacionit — token i revokuar pa zevendesues — do nxirrte userin jashte.
    /// </summary>
    Task RotateAsync(RefreshToken oldToken, RefreshToken newToken, CancellationToken cancellationToken = default);

    /// <summary>Logout, dhe gjithashtu reagimi ndaj riperdorimit te nje token-i te revokuar.</summary>
    Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fshin token-et e skaduar. Kthen numrin e rreshtave, qe job-i te mund ta logoje.
    /// Pa kete, tabela rritet pa fund: cdo login dhe cdo refresh shton nje rresht.
    /// </summary>
    Task<int> DeleteExpiredAsync(DateTime cutoffUtc, CancellationToken cancellationToken = default);
}

using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AssetManagementSystem.Application.Services;

public sealed class RefreshTokenCleanupJob : IRefreshTokenCleanupJob
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<RefreshTokenCleanupJob> _logger;

    public RefreshTokenCleanupJob(
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<RefreshTokenCleanupJob> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Pa try/catch: nese deshton, Hangfire duhet ta SHOHE, t'i beje retry dhe ta
        // shfaqe ne dashboard. Nje catch qe vetem logon do e fshehte deshtimin.
        var deleted = await _refreshTokenRepository
            .DeleteExpiredAsync(DateTime.UtcNow, cancellationToken);

        if (deleted == 0)
        {
            _logger.LogInformation("Refresh token cleanup: nothing to delete.");
            return;
        }

        _logger.LogInformation(
            "Refresh token cleanup: deleted {Count} expired tokens.", deleted);
    }
}

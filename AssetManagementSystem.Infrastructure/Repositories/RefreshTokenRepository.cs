using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace AssetManagementSystem.Infrastructure.Repositories;
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;
    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    // Pa AsNoTracking: rreshti i lexuar do te revokohet, pra duhet i gjurmuar.
    public async Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);
    }
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task RotateAsync(
        RefreshToken oldToken,
        RefreshToken newToken,
        CancellationToken cancellationToken = default)
    {
        _context.RefreshTokens.Add(newToken);
        oldToken.RevokedAtUtc = DateTime.UtcNow;
        oldToken.ReplacedByTokenId = newToken.Id;
        // Nje SaveChanges per te dyja: ose ndodhin bashke, ose asnjera.
        await _context.SaveChangesAsync(cancellationToken);
    }
    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        // ExecuteUpdate: nje UPDATE i vetem, pa i terhequr rreshtat ne memorie.
        // UpdatedAt/UpdatedBy mbushen nga AuditExecuteUpdateInterceptor.
        await _context.RefreshTokens
            .Where(token => token.UserId == userId && token.RevokedAtUtc == null)
            .ExecuteUpdateAsync(
                setter => setter.SetProperty(token => token.RevokedAtUtc, now),
                cancellationToken);
    }
    public async Task<int> DeleteExpiredAsync(
        DateTime cutoffUtc,
        CancellationToken cancellationToken = default)
    {
        // ExecuteDelete: nje DELETE i vetem. Me te terhequr rreshtat ne memorie dhe
        // Remove() per secilin, 100,000 token-e do kerkonin 100,000 objekte te gjurmuar.
        //
        // Nuk filtrohet mbi RevokedAtUtc: nje token i revokuar POR i paskaduar duhet te
        // mbetet, sepse mbi te bazohet zbulimi i riperdorimit te nje token-i te vjedhur.
        return await _context.RefreshTokens
            .Where(token => token.ExpiresAtUtc < cutoffUtc)
            .ExecuteDeleteAsync(cancellationToken);
    }
}

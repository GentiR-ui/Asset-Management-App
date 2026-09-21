using AssetManagementSystem.Domain.Interfaces;
using ErrorOr;

namespace AssetManagementSystem.Infrastructure;
public sealed class TransactionRunner : ITransactionRunner
{
    private readonly ApplicationDbContext _context;

    public TransactionRunner(ApplicationDbContext context) => _context = context;

    public async Task<ErrorOr<T>> ExecuteAsync<T>(Func<Task<ErrorOr<T>>> operation, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await operation();

            if (result.IsError)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                _context.ChangeTracker.Clear();
                return result;
            }

            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            _context.ChangeTracker.Clear();
            throw;
        }
    }
}

using ErrorOr;

namespace AssetManagementSystem.Domain.Interfaces;
public interface ITransactionRunner
{
    Task<ErrorOr<T>> ExecuteAsync<T>(Func<Task<ErrorOr<T>>> operation, CancellationToken cancellationToken = default);
}

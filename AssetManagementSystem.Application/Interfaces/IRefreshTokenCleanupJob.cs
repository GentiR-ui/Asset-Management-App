namespace AssetManagementSystem.Application.Interfaces;
/// <summary>
/// Pastrimi i refresh token-eve te skaduar. Job i dyte i Hangfire, bashke me
/// IWarrantyExpireJob — dhe shembull i nje job-i "mirembajtje": pa email, pa user.
/// </summary>
public interface IRefreshTokenCleanupJob
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}

namespace AssetManagementSystem.Application.Interfaces;

/// <summary>
/// Job-i i njoftimit per garanci qe skadojne. Thirret nga Hangfire sipas orarit,
/// por eshte metode e zakonshme — mund te thirret edhe nga kudo tjeter.
///
/// Ekziston si interface sepse RecurringJob.AddOrUpdate&lt;T&gt; e zgjidh T nga DI-ja;
/// me interface, Hangfire ruan nje "recete" ndaj kontrates, jo ndaj implementimit.
/// </summary>
public interface IWarrantyExpireJob
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}

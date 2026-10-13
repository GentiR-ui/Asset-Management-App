using System.Text;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;
namespace AssetManagementSystem.Application.Services;
/// <summary>
/// Zevendesuesi i WarrantyExpireBackgroundService.
///
/// Dallimi nuk eshte ne logjike — logjika eshte e njejta. Dallimi eshte se kjo klase
/// NUK e mban orarin: nuk ka while, nuk ka Task.Delay, nuk ka IServiceScopeFactory.
/// Hangfire-i e thërret kur t'i vije radha, dhe scope-in e kerkeses e krijon vete,
/// pra varesite hyjne ne konstruktor si ne cdo service tjeter.
/// </summary>
public sealed class WarrantyExpireJob : IWarrantyExpireJob
{
    private const int DaysThreshold = 30;
    private const string NotificationRecipientEmail = "admin@company.com";
    private readonly IAssetRepository _assetRepository;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<WarrantyExpireJob> _logger;
    public WarrantyExpireJob(
        IAssetRepository assetRepository,
        IEmailSender emailSender,
        ILogger<WarrantyExpireJob> logger)
    {
        _assetRepository = assetRepository;
        _emailSender = emailSender;
        _logger = logger;
    }
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Checking for assets with warranties expiring within {Days} days...", DaysThreshold);
        var expiringAssets = await _assetRepository
            .GetExpiringWarrantiesAsync(DaysThreshold, cancellationToken);
        if (expiringAssets.Count == 0)
        {
            _logger.LogInformation("No expiring warranties found.");
            return;
        }
        _logger.LogWarning(
            "Found {Count} assets with expiring warranties. Sending email alert...",
            expiringAssets.Count);
        var emailBody = new StringBuilder();
        emailBody.AppendLine("Përshëndetje,\n");
        emailBody.AppendLine(
            $"U gjetën {expiringAssets.Count} asete me garancion që skadon brenda {DaysThreshold} ditëve të ardhshme:\n");
        foreach (var asset in expiringAssets)
        {
            // Pa ?? dhe pa ?.: read model-i i garanton te dyja fushat, sepse query-ja
            // i kerkoi vete. Nje fallback si "Pa departament" fsheh gabim, nuk e zgjidh.
            emailBody.AppendLine(
                $"- [{asset.AssetTag}] {asset.Name} "
                + $"| Garancia: {asset.WarrantyExpiryDate:yyyy-MM-dd} "
                + $"| Departamenti: {asset.DepartmentName}");
        }
        emailBody.AppendLine("\nJu lutemi merrni masat e nevojshme me furnitorët.");
        await _emailSender.SendAsync(
            to: NotificationRecipientEmail,
            subject: $"[KUJDES] {expiringAssets.Count} Asete me Garanci afër Skadimit",
            body: emailBody.ToString());
        _logger.LogInformation(
            "Warranty alert email successfully sent to {Email}.", NotificationRecipientEmail);
    }
}

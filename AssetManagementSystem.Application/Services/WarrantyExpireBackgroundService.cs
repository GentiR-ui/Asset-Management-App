using System.Text;
using AssetManagementSystem.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AssetManagementSystem.Application.Services;

public sealed class WarrantyExpireBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WarrantyExpireBackgroundService> _logger;

    
    private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(24);
    private const int DaysThreshold = 30;
    private const string NotificationRecipientEmail = "admin@company.com";

    public WarrantyExpireBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<WarrantyExpireBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WarrantyExpireBackgroundService started.");

        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndAlertExpiringWarrantiesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                
                break;
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "An error occurred while checking expiring warranties.");
            }

            try
            {
                
                await Task.Delay(CheckInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                
                break;
            }
        }

        _logger.LogInformation("WarrantyExpireBackgroundService stopped.");
    }

    private async Task CheckAndAlertExpiringWarrantiesAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking for assets with warranties expiring within {Days} days...", DaysThreshold);

        
        using var scope = _scopeFactory.CreateScope();

        var assetRepository = scope.ServiceProvider.GetRequiredService<IAssetRepository>();
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

        var expiringAssets = await assetRepository.GetExpiringWarrantiesAsync(DaysThreshold, cancellationToken);

        if (expiringAssets.Count == 0)
        {
            _logger.LogInformation("No expiring warranties found.");
            return;
        }

        _logger.LogWarning("Found {Count} assets with expiring warranties. Sending email alert...", expiringAssets.Count);


       
        var emailBody = new StringBuilder();
        emailBody.AppendLine($"Përshëndetje,\n");
        emailBody.AppendLine($"U gjetën {expiringAssets.Count} asete me garancion që skadon brenda {DaysThreshold} ditëve të ardhshme:\n");

        foreach (var asset in expiringAssets)
        {
            var expiry = asset.WarrantyExpiryDate?.ToString("yyyy-MM-dd") ?? "N/A";
            var dept = asset.Department?.Name ?? "Pa departament";
            emailBody.AppendLine($"- [{asset.AssetTag}] {asset.Name} | Garancia: {expiry} | Departamenti: {dept}");
        }

        emailBody.AppendLine("\nJu lutemi merrni masat e nevojshme me furnitorët.");

        
        await emailSender.SendAsync(
            to: NotificationRecipientEmail,
            subject: $"[KUJDES] {expiringAssets.Count} Asete me Garanci afër Skadimit",
            body: emailBody.ToString());

        _logger.LogInformation("Warranty alert email successfully sent to {Email}.", NotificationRecipientEmail);
    }
}

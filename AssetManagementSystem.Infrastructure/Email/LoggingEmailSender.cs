using AssetManagementSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

public sealed class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger) => _logger = logger;

    public Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        _logger.LogWarning("\n=== EMAIL ===\nTo: {To}\nSubject: {Subject}\n{Body}\n=============",
            to, subject, body);
        return Task.CompletedTask;
    }
}

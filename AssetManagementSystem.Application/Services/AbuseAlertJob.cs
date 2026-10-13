using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AssetManagementSystem.Application.Services;

public sealed class AbuseAlertJob : IAbuseAlertJob
{
    private const string NotificationRecipientEmail = "admin@company.com";

    private readonly IIdentityProvider _identityProvider;
    private readonly IEmailSender _emailSender;
    private readonly IFeedbackGenerator _feedbackGenerator;
    private readonly ILogger<AbuseAlertJob> _logger;

    public AbuseAlertJob(
        IIdentityProvider identityProvider,
        IEmailSender emailSender,
        IFeedbackGenerator feedbackGenerator,
        ILogger<AbuseAlertJob> logger)
    {
        _identityProvider = identityProvider;
        _emailSender = emailSender;
        _feedbackGenerator = feedbackGenerator;
        _logger = logger;
    }

    public async Task ExecuteAsync(
        Guid userId,
        string endpoint,
        int requestCount,
        CancellationToken cancellationToken)
    {
        // Useri lexohet nga databaza, jo nga HttpContext: ky kod nuk ekzekutohet
        // brenda nje kerkese HTTP, pra nuk ka user "aktual".
        var user = await _identityProvider.FindByIdAsync(userId);

        var who = user is null
            ? userId.ToString()
            : $"{user.FirstName} {user.LastName} ({user.Email})";

        // ── AI-ja, me rrjete sigurie ────────────────────────────────────────────
        // Thirrja behet KETU, ne prapavije: kerkesa HTTP eshte pergjigjur me kohe.
        // generated == null nese mungon celesi, nese Gemini s'arrihet, ose nese
        // filtrat e tij e bllokojne. Ne cdo rast, alerti dergohet gjithsesi.
        var generated = await _feedbackGenerator.GenerateAsync(endpoint, requestCount, cancellationToken);

        var message = generated
            ?? $"Sjellje e pazakonte: {requestCount} kerkesa GET {endpoint} brenda 60 sekondave.";

        var body =
            $"{message}\n\n"
            + $"Useri:     {who}\n"
            + $"Endpoint:  GET {endpoint}\n"
            + $"Kerkesa:   {requestCount} brenda 60 sekondave\n\n"
            + "Shenim: numri i larte i kerkesave nuk nenkupton domosdoshmerisht abuzim. "
            + "Ky eshte njoftim vezhgimi, jo bllokim — API-ja nuk ka refuzuar asnje kerkese.";

        await _emailSender.SendAsync(
            to: NotificationRecipientEmail,
            subject: $"[VEZHGIM] {requestCount} kerkesa ne {endpoint}",
            body: body);

        _logger.LogInformation(
            "Alert i derguar: {Count} kerkesa GET {Endpoint} nga {UserId}. AI: {AiUsed}.",
            requestCount, endpoint, userId, generated is null ? "jo" : "po");
    }
}

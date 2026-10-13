using System.Security.Claims;
using AssetManagementSystem.Application.Interfaces;
using Hangfire;

namespace AssetManagementSystem.API.Middleware;

public sealed class RequestCounterMiddleware
{
    private const int Threshold = 5;
    private static readonly TimeSpan Window = TimeSpan.FromSeconds(60);

    private readonly RequestDelegate _next;

    public RequestCounterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, RequestRateCounter counter)
    {
        if (context.User.Identity?.IsAuthenticated == true
            && HttpMethods.IsGet(context.Request.Method)
            && context.Request.Path.StartsWithSegments("/api")
            && Guid.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            var path = context.Request.Path.Value ?? string.Empty;
            var count = counter.Increment($"{userId}:{path}", Window);

            // SAKTESISHT ==: alerti niset nje here per dritare, jo per cdo kerkese pasuese.
            if (count == Threshold)
            {
                // Fire-and-forget: Enqueue vetem SHKRUAN nje rresht ne databazen e Hangfire
                // dhe kthehet. Email-i dhe leximi i userit ndodhin ne nje worker tjeter,
                // pra kerkesa HTTP nuk pret asgje.
                BackgroundJob.Enqueue<IAbuseAlertJob>(
                    job => job.ExecuteAsync(userId, path, count, CancellationToken.None));
            }
        }

        await _next(context);
    }
}

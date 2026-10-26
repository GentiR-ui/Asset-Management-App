using System.Security.Claims;
using System.Threading.RateLimiting;
using AssetManagementSystem.API.Constants;
using Microsoft.AspNetCore.RateLimiting;

namespace AssetManagementSystem.API.Extensions;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // 1. Politika per Auth (Login/Forgot-Password): Bazohet ne IP (5 thirrje ne minute)
            options.AddPolicy(RateLimitPolicies.AuthPolicy, httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter($"auth:{ip}", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                });
            });

            // 2. Politika e pergjithshme: Bazohet ne UserId nese eshte i kycur, perndryshe ne IP
            options.AddPolicy(RateLimitPolicies.GeneralPolicy, httpContext =>
            {
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var key = !string.IsNullOrWhiteSpace(userId)
                    ? $"user:{userId}"
                    : $"ip:{httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";

                return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 60,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                });
            });

            // Mesazh i qarte kur klienti refuzohet me 429
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsync(
                    "{\"success\":false,\"statusCode\":429,\"message\":\"Too many requests. Please try again later.\"}",
                    token);
            };
        });

        return services;
    }
}

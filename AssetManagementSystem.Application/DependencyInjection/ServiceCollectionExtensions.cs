using AssetManagementSystem.Application.Common.Authorization;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Application.Services;
using AssetManagementSystem.Application.Validators;
using AssetManagementSystem.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace AssetManagementSystem.Application.DependencyInjection;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAssetService, AssetService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IAssetScopeResolver, AssetScopeResolver>();
        // Singleton: CurrentUserService nuk ruan asnje fushe — userin e llogarit nga
        // IHttpContextAccessor ne cdo thirrje. Duhet Singleton sepse
        // AuditExecuteUpdateInterceptor (Singleton) varet nga ai: nje Singleton nuk
        // mund te konsumoje Scoped (captive dependency) dhe .NET-i nuk niset fare.
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        // Job-i i garancive: Scoped, sepse Hangfire krijon nje scope per cdo ekzekutim.
        services.AddScoped<IWarrantyExpireJob, WarrantyExpireJob>();
        services.AddScoped<IRefreshTokenCleanupJob, RefreshTokenCleanupJob>();
        // JASHTE PERDORIMIT: oraret i mban Hangfire tani (shih Program.cs).
        // Klasa WarrantyExpireBackgroundService mbetet ne kod vetem per krahasim;
        // nese regjistrohet bashke me job-in, njoftimi dergohet DY here.
        // services.AddHostedService<WarrantyExpireBackgroundService>();
        var redisConnection = configuration.GetConnectionString("Redis");
        if (string.IsNullOrWhiteSpace(redisConnection))
        {
            // Pa Redis te konfiguruar, aplikacioni duhet te niset gjithsesi.
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "ams:";
            });
        }
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IAbuseAlertJob, AbuseAlertJob>();

        return services;
    }
}

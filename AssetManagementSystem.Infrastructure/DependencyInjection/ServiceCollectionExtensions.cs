using System.Text;
using Hangfire;
using Hangfire.SqlServer;
using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Interfaces;
using AssetManagementSystem.Infrastructure.Authorization;
using AssetManagementSystem.Infrastructure.Identity;
using AssetManagementSystem.Infrastructure.Interceptors;
using AssetManagementSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using AssetManagementSystem.Infrastructure.Ai;
namespace AssetManagementSystem.Infrastructure.DependencyInjection;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentity<User, UserRole>(
            options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            }
        )
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        AddJwtAuthentication(services, configuration);
        services.AddScoped<IIdentityProvider, IdentityProvider>();
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IEmailSender, LoggingEmailSender>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<ITransactionRunner, TransactionRunner>();
        services.AddScoped<IScopeContext, ScopeContext>();
        services.AddHttpContextAccessor();
        services.AddHttpClient<IFeedbackGenerator, GroqFeedbackGenerator>()
            .ConfigureHttpClient(client => client.Timeout = TimeSpan.FromSeconds(20));

        // Singleton: interceptori nuk mban gjendje — userin e lexon nga IHttpContextAccessor
        // ne momentin e thirrjes. Si Scoped, EF-i do shihte opsione te reja per cdo kerkese
        // dhe do ndertonte nje service provider te brendshem te re — deri ne crash te 21-tin.
        // Singleton: pa gjendje, lexon IHttpContextAccessor ne cdo thirrje.
        // Duhet Singleton sepse interceptori — qe eshte Singleton — e konsumon.
        services.AddSingleton<CurrentUserAccessor>();

        services.AddSingleton<AuditExecuteUpdateInterceptor>();
        // Regjistrimi i VETEM i DbContext-it: nje burim i vertetes per konfigurimin.
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(
                    sp.GetRequiredService<AuditExecuteUpdateInterceptor>());
        });
        AddHangfireProcessing(services, configuration);
        return services;
    }
    /// <summary>
    /// Hangfire ruan gjendjen e job-eve ne SQL Server — ne te njejten databaze.
    /// Pikerisht kjo e ndan nga BackgroundService: orari dhe historiku i mbijetojne
    /// rinisjes se aplikacionit, sepse nuk jetojne ne memorie.
    ///
    /// Skema [HangFire] krijohet vetvetiu ne nisjen e pare — pa migrim, pa SQL me dore.
    /// </summary>
    private static void AddHangfireProcessing(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(
                configuration.GetConnectionString("DefaultConnection"),
                new SqlServerStorageOptions
                {
                    // Zero: merr job-in sapo shfaqet, pa pritur ciklin e radhes.
                    QueuePollInterval = TimeSpan.Zero,
                    // Nese procesi bie mes ekzekutimit, job-i rikthehet ne rend pas 5 minutash.
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    UseRecommendedIsolationLevel = true,
                    // Pa lock global: lejon disa servera Hangfire paralel pa bllokime ne tabela.
                    DisableGlobalLocks = true
                }));
        // Serveri eshte ai qe EKZEKUTON. Pa te, job-et ruhen ne databaze dhe presin pa fund.
        services.AddHangfireServer();
    }
    private static void AddJwtAuthentication(
    IServiceCollection services,
    IConfiguration configuration)
        {
            var section = configuration.GetSection(JwtSettings.SectionName);
            services.Configure<JwtSettings>(section);
            var settings = section.Get<JwtSettings>()
                ?? throw new InvalidOperationException(
                    $"Missing configuration section '{JwtSettings.SectionName}'.");
            if (string.IsNullOrWhiteSpace(settings.Key))
            {
                throw new InvalidOperationException(
                    "JWT signing key is not configured. Set it with: " +
                    "dotnet user-secrets set \"JwtSettings:Key\" \"<a long random secret>\"");
            }
            services.AddScoped<ITokenService, JwtTokenService>();
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = settings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = settings.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(settings.Key)),
                        ClockSkew = TimeSpan.Zero
                    };
                });
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }
}

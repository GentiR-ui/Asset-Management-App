using System.Text;
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
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<ITransactionRunner, TransactionRunner>();
        services.AddScoped<IScopeContext, ScopeContext>();
        services.AddHttpContextAccessor();

        // Singleton: interceptori nuk mban gjendje — userin e lexon nga IHttpContextAccessor
        // ne momentin e thirrjes. Si Scoped, EF-i do shihte opsione te reja per cdo kerkese
        // dhe do ndertonte nje service provider te brendshem te re — deri ne crash te 21-tin.
        services.AddSingleton<AuditExecuteUpdateInterceptor>();

        // Regjistrimi i VETEM i DbContext-it: nje burim i vertetes per konfigurimin.
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(
                    sp.GetRequiredService<AuditExecuteUpdateInterceptor>());
        });





        



        

        return services;
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


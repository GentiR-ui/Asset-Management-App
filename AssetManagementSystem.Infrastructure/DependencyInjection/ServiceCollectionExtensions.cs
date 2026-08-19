using System.Text;
using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Interfaces;
using AssetManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

                
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
        services.AddScoped<IEmailSender, LoggingEmailSender>();
        

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


using AssetManagementSystem.Application.Caching;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Application.Services;
using AssetManagementSystem.Application.Validators;
using AssetManagementSystem.Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
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

        return services;
        
       
    }
}

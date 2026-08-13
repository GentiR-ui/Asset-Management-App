using AssetManagementSystem.Domain.Common;
using AssetManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AssetManagementSystem.Infrastructure;

public static class DatabaseSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();

        foreach (var roleName in AppRoles.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            await roleManager.CreateAsync(new UserRole { Name = roleName });
        }
    }
}

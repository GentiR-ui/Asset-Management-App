
using Microsoft.AspNetCore.Builder;

namespace AssetManagementSystem.Infrastructure.DependencyInjection;

public static class WebApplicationExtensions
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        await DatabaseSeeder.SeedAsync(app.Services);
    }
}

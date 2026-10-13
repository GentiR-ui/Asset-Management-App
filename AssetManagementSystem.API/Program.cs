using AssetManagementSystem.API.Filters;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.API.Middleware;
using AssetManagementSystem.Application.DependencyInjection;
using AssetManagementSystem.Infrastructure.DependencyInjection;
using Hangfire;
using Hangfire.Dashboard;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<ExceptionFilter>();
})
// Enum-et udhetojne si EMRA, jo numra: { "category": "Laptop" } ne vend te { "category": 0 }
.AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
    });
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<RequestRateCounter>();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.SeedDatabaseAsync();
}
app.UseHttpsRedirection();
// PARA UseAuthorization: FallbackPolicy (RequireAuthenticatedUser) zbatohet per cdo
// kerkese pa endpoint, dhe /hangfire eshte middleware, pa endpoint -> do kthente 401.
// Dashboard-i ka autorizimin e vet, pra nuk i duhet ai i aplikacionit.
//
// LocalRequestsOnly: pa filter, dashboard-i lejon KEDO te ekzekutoje ose fshije job-e.
// Nuk vendoset filter me rol sepse autentikimi i projektit eshte JWT: shfletuesi nuk
// dergon token kur hap /hangfire, pra nje filter [Authorize] do te bllokonte edhe adminin.
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new LocalRequestsOnlyAuthorizationFilter()]
});
app.UseAuthentication();
app.UseAuthorization();
// Pas autentikimit: scope-i varet nga useri i identifikuar.
app.UseMiddleware<ScopeMiddleware>();

app.UseMiddleware<RequestCounterMiddleware>();
// PAS Build(): AddOrUpdate shkruan ne storage, dhe storage-i konfigurohet gjate Build().
// AddOrUpdate eshte idempotent — e njejta Id thjesht e perditeson orarin, nuk shton dyfish.
// CancellationToken.None zevendesohet nga Hangfire ne runtime me token-in e vertete.
RecurringJob.AddOrUpdate<IWarrantyExpireJob>(
    recurringJobId: "warranty-expiry-check",
    methodCall: job => job.ExecuteAsync(CancellationToken.None),
    cronExpression: Cron.Daily(8),
    options: new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
// Mirembajtje: tabela RefreshTokens rritet me cdo login dhe cdo refresh.
// 03:00 me qellim — ora me e qete, sepse DELETE-i mban lock-e ne tabele.
RecurringJob.AddOrUpdate<IRefreshTokenCleanupJob>(
    recurringJobId: "refresh-token-cleanup",
    methodCall: job => job.ExecuteAsync(CancellationToken.None),
    cronExpression: Cron.Daily(3),
    options: new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
app.MapControllers();
app.Run();

using AssetManagementSystem.API.Filters;
using AssetManagementSystem.API.OpenApi;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.API.Middleware;
using AssetManagementSystem.Application.DependencyInjection;
using AssetManagementSystem.Infrastructure.DependencyInjection;
using AssetManagementSystem.API.Extensions;
using Hangfire;
using Hangfire.Dashboard;
using System.Text.Json.Serialization;
using System.Security.Claims;
using System.Threading.RateLimiting;
using AssetManagementSystem.API.Constants;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
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
    })
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true; // <--Ky kod i thotë ASP.NET: “Mos i trajto vetë gabimet e modelit (si email jo valid) në endpointet e mia. Lëre atë punë krejtësisht në duart e ValidationFilter dhe ExceptionFilter.”
});

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCustomRateLimiter();
builder.Services.AddSingleton<RequestRateCounter>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // AllowAnonymous: FallbackPolicy kerkon user te autentikuar per cdo endpoint,
    // dhe dokumenti OpenAPI duhet te lexohet para se te besh login.
    app.MapOpenApi().AllowAnonymous();

    // PARA UseAuthentication, per te njejten arsye si dashboard-i i Hangfire:
    // /swagger nuk eshte endpoint, pra FallbackPolicy do e ndalonte me 401.
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Asset Management API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Asset Management API";
    });
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
// UseRateLimiter pas UseAuthorization: politika GeneralPolicy perdor UserId nga JWT,
// pra rate limiter duhet te ekzekutohet pasi autentikimi ta kete ngarkuar User-in.
app.UseRateLimiter();
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

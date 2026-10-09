using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Interfaces;

namespace AssetManagementSystem.API.Middleware;

/// <summary>
/// Zgjidh scope-in nje here per kerkese dhe e ruan te IScopeContext, qe ApplicationDbContext
/// ta lexoje sinkronisht kur nderton global query filter-in.
/// Duhet te vije PAS UseAuthentication: pa identitet te verifikuar s'ka cfare te zgjidhet.
/// </summary>
public sealed class ScopeMiddleware
{
    private readonly RequestDelegate _next;

    public ScopeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IAssetScopeResolver scopeResolver,
        IScopeContext scopeContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            try
            {
                var scope = await scopeResolver.ResolveScopeAsync(context.RequestAborted);

                if (scope.IsGlobal)
                {
                    scopeContext.SetGlobal();
                }
                else
                {
                    scopeContext.SetDepartment(scope.DepartmentId!.Value);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // User i autentikuar por pa punonjes. Endpoint-et e mbrojtura e ndalojne me [Authorize].
                scopeContext.SetGlobal();
            }
        }

        await _next(context);
    }
}

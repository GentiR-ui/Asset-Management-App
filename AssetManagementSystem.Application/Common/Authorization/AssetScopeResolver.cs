using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Common;
using AssetManagementSystem.Domain.Interfaces;

namespace AssetManagementSystem.Application.Common.Authorization;

/// <summary>
/// Departamenti NUK vjen nga token-i. Token-i eshte nje fotografi e castit te login-it,
/// ndersa scope-i eshte autorizim dhe duhet te jete i tanishem. Lexohet nga databaza,
/// me cache te shkurter per user dhe invalidim kur punonjesi nderron departament.
/// </summary>
public sealed class AssetScopeResolver : IAssetScopeResolver
{
    private static readonly TimeSpan ScopeTtl = TimeSpan.FromSeconds(60);

    private readonly ICurrentUserService _currentUser;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICacheService _cacheService;

    public AssetScopeResolver(
        ICurrentUserService currentUser,
        IEmployeeRepository employeeRepository,
        ICacheService cacheService)
    {
        _currentUser = currentUser;
        _employeeRepository = employeeRepository;
        _cacheService = cacheService;
    }

    public static string CacheKeyFor(Guid userId) => $"user-scope:{userId}";

    public async Task<AssetQueryScope> ResolveScopeAsync(CancellationToken cancellationToken = default)
    {
        if (string.Equals(_currentUser.Role, AppRoles.Admin, StringComparison.OrdinalIgnoreCase))
        {
            return AssetQueryScope.Global();
        }

        var cacheKey = CacheKeyFor(_currentUser.UserId);

        var cached = await _cacheService.GetAsync<Guid?>(cacheKey, cancellationToken);

        if (cached is not null)
        {
            return AssetQueryScope.Department(cached.Value);
        }

        var employee = await _employeeRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken);

        if (employee is null)
        {
            throw new UnauthorizedAccessException("User is not assigned to a department.");
        }

        await _cacheService.SetAsync(cacheKey, (Guid?)employee.DepartmentId, ScopeTtl, cancellationToken);

        return AssetQueryScope.Department(employee.DepartmentId);
    }
}

using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Common;

namespace AssetManagementSystem.Application.Common.Authorization;

public sealed class AssetScopeResolver : IAssetScopeResolver
{
    private readonly ICurrentUserService _currentUser;

    public AssetScopeResolver(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public AssetQueryScope ResolveScope()
    {
        if (string.Equals(_currentUser.Role, AppRoles.Admin, StringComparison.OrdinalIgnoreCase))
        {
            return AssetQueryScope.Global();
        }

        if (_currentUser.DepartmentId is null)
        {
            throw new UnauthorizedAccessException("User is not assigned to a department.");
        }

        return AssetQueryScope.Department(_currentUser.DepartmentId.Value);
    }
}

using AssetManagementSystem.Application.Common.Authorization;

namespace AssetManagementSystem.Application.Interfaces;

public interface IAssetScopeResolver
{
    AssetQueryScope ResolveScope();
}

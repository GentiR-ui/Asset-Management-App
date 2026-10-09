using AssetManagementSystem.Application.Common.Authorization;

namespace AssetManagementSystem.Application.Interfaces;

public interface IAssetScopeResolver
{
    Task<AssetQueryScope> ResolveScopeAsync(CancellationToken cancellationToken = default);
}

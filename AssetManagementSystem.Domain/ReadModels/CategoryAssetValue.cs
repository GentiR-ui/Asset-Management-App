using AssetManagementSystem.Domain.Enums;

namespace AssetManagementSystem.Domain.ReadModels;

public sealed record CategoryAssetValue(
    AssetCategory Category,
    int AssetCount,
    decimal TotalValue
);

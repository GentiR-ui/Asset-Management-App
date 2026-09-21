using AssetManagementSystem.Domain.Enums;

namespace AssetManagementSystem.Domain.ReadModels;

/// <summary>Status mbetet enum: perkthimi ne string behet ne Application, jo ne SQL.</summary>
public sealed record StatusAssetValue(
    AssetStatus Status,
    int AssetCount,
    decimal TotalValue
);

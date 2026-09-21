namespace AssetManagementSystem.Domain.ReadModels;

/// <summary>Numri dhe vlera totale e aseteve, te dyja nga nje query e vetme.</summary>
public sealed record AssetValueSummary(
    int AssetCount,
    decimal TotalValue
);

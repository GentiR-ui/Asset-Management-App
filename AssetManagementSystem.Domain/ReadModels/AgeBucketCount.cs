namespace AssetManagementSystem.Domain.ReadModels;

/// <summary>
/// BucketIndex: 0 = me i riu, 3 = me i vjetri. Etiketa ("0-1", "5+") ndertohet
/// ne Application, sepse varet nga kufijte qe zgjodhi ajo shtrese.
/// </summary>
public sealed record AgeBucketCount(
    int BucketIndex,
    int AssetCount,
    decimal TotalValue
);

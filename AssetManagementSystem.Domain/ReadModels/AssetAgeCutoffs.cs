namespace AssetManagementSystem.Domain.ReadModels;

/// <summary>
/// Datat kufi qe ndajne asetet ne kater grupe moshe. Kush i llogarit vendos politiken
/// (sa vjet zgjat nje cikel rifreskimi); repository-ja vetem grupon sipas tyre.
/// </summary>
public sealed record AssetAgeCutoffs(
    DateTime First,
    DateTime Second,
    DateTime Third
);

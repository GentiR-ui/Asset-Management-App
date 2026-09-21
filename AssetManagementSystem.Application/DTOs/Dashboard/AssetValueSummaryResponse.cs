namespace AssetManagementSystem.Application.DTOs.Dashboard;

/// <summary>Vlera totale e blerjes per te gjitha asetet, plus sa asete jane.</summary>
public sealed record AssetValueSummaryResponse
{
    public required int AssetCount { get; init; }
    public required decimal TotalValue { get; init; }
}

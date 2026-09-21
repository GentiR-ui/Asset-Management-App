namespace AssetManagementSystem.Application.DTOs.Dashboard;

public sealed record CategoryValueResponse
{
    public required string Category { get; init; }
    public required int AssetCount { get; init; }
    public required decimal TotalValue { get; init; }
}

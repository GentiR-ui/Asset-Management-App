namespace AssetManagementSystem.Application.DTOs.Dashboard;

public sealed record AgeValueResponse
{

    public required string AgeRange { get; init; }
    public required int AssetCount { get; init; }
    public required decimal TotalValue { get; init; }
}

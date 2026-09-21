namespace AssetManagementSystem.Application.DTOs.Dashboard;

/// <summary>Statuset pa asnje aset nuk shfaqen: GroupBy kthen vetem ato qe ekzistojne.</summary>
public sealed record StatusValueResponse
{
    public required string Status { get; init; }
    public required int AssetCount { get; init; }
    public required decimal TotalValue { get; init; }
}

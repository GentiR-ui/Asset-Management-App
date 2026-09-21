namespace AssetManagementSystem.Application.DTOs.Dashboard;

/// <summary>Nje rresht per departament. Shuma e ketyre rreshtave duhet te perputhet me AssetValueSummaryResponse.</summary>
public sealed record DepartmentValueResponse
{
    /// <summary>Null kur asetet nuk i jane caktuar askujt — grupi "i pacaktuar".</summary>
    public Guid? DepartmentId { get; init; }
    public string? DepartmentName { get; init; }

    public required int AssetCount { get; init; }
    public required decimal TotalValue { get; init; }
}

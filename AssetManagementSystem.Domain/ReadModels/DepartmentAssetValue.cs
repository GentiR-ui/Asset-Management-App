namespace AssetManagementSystem.Domain.ReadModels;

/// <summary>DepartmentId dhe DepartmentName jane null per asetet qe s'i jane caktuar askujt.</summary>
public sealed record DepartmentAssetValue(
    Guid? DepartmentId,
    string? DepartmentName,
    int AssetCount,
    decimal TotalValue
);

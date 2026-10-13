namespace AssetManagementSystem.Application.DTOs.Dashboard;
public sealed record DepartmentValueResponse
{
    /// <summary>Null kur asetet nuk i jane caktuar askujt — grupi "i pacaktuar".</summary>
    public Guid? DepartmentId { get; init; }
    public string? DepartmentName { get; init; }
    /// <summary>Sa asete i perkasin departamentit.</summary>
    public required int AssetCount { get; init; }

    /// <summary>Sa prej tyre jane te caktuara nje punonjesi.</summary>
    public required int AssignedCount { get; init; }

    /// <summary>Fushe e llogaritur — klienti s'ka pse ta zbrese vete.</summary>
    public int AvailableCount => AssetCount - AssignedCount;
    public required decimal TotalValue { get; init; }
}

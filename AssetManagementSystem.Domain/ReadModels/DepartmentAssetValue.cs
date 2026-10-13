namespace AssetManagementSystem.Domain.ReadModels;
/// <summary>DepartmentId dhe DepartmentName jane null per asetet qe s'i jane caktuar askujt.</summary>
public sealed record DepartmentAssetValue(
    Guid? DepartmentId,
    string? DepartmentName,

    /// <summary>Sa asete i PERKASIN departamentit. Nuk ndryshon kur hiqet nje caktim —
    /// nje laptop i paleshuar ende i perket IT-se.</summary>
    int AssetCount,

    /// <summary>Sa prej tyre jane aktualisht te caktuara nje punonjesi.</summary>
    int AssignedCount,

    decimal TotalValue
);

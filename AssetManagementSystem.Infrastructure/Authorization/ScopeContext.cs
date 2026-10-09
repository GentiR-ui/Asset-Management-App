using AssetManagementSystem.Domain.Interfaces;

namespace AssetManagementSystem.Infrastructure.Authorization;

/// <summary>
/// Scoped: nje instance per kerkese HTTP. Parazgjedhja eshte globale, sepse kerkesat
/// qe nuk kalojne nga middleware-i (login, swagger) nuk prekin te dhena te kufizuara.
/// </summary>
public sealed class ScopeContext : IScopeContext
{
    public bool IsGlobal { get; private set; } = true;

    public Guid? DepartmentId { get; private set; }

    public void SetGlobal()
    {
        IsGlobal = true;
        DepartmentId = null;
    }

    public void SetDepartment(Guid departmentId)
    {
        IsGlobal = false;
        DepartmentId = departmentId;
    }
}

namespace AssetManagementSystem.Application.Common.Authorization;
public sealed record AssetQueryScope
{
    public bool IsGlobal { get; }
    public Guid? DepartmentId { get; }

    private AssetQueryScope(bool isGlobal, Guid? departmentId)
    {
        IsGlobal = isGlobal;
        DepartmentId = departmentId;
    }

    public static AssetQueryScope Global() => new(true, null);
    public static AssetQueryScope Department(Guid departmentId) => new(false, departmentId);
}

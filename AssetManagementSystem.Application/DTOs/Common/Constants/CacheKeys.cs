namespace AssetManagementSystem.Application.Common.Constants;

public static class CacheKeys
{
    public const string TotalValue = "dashboard:total-value";
    public const string ByDepartment = "dashboard:by-department";
    public const string ByStatus = "dashboard:by-status";
    public const string ByCategory = "dashboard:by-category";
    public const string Age = "dashboard:age";

    
    public static readonly string[] AllDashboardKeys = 
    [
        TotalValue, 
        ByDepartment, 
        ByStatus, 
        ByCategory, 
        Age
    ];
}
namespace AssetManagementSystem.Domain.Common;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string ItManager = "IT-Manager";
    public const string Employee = "Employee";

    public static readonly string[] All = [Admin, ItManager, Employee];
}
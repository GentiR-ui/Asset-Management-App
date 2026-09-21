namespace AssetManagementSystem.Domain.Common;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string ItManager = "IT-Manager";
    public const string Employee = "Employee";

    /// <summary>Llogari e vete-regjistruar. Nuk eshte staf: nuk ka rresht te Employees.</summary>
    public const string Client = "Client";

    /// <summary>Rolet qe nenkuptojne staf, pra qe kerkojne rresht te Employees.</summary>
    public static readonly string[] Staff = [Employee, ItManager];

    public static readonly string[] All = [Admin, ItManager, Employee, Client];
}
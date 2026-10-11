namespace AssetManagementSystem.Domain.ReadModels;

/// <summary>
/// Forma e sheshte e nje punonjesi per LEXIM. Emri, mbiemri dhe email-i vijne nga llogaria
/// e lidhur — dhe kjo ishte arsyeja pse metodat e leximit benin Include(employee.User),
/// qe terhiqte 17 kolona, perfshire PasswordHash dhe SecurityStamp, per tre fusha.
/// </summary>
public sealed record EmployeeListItem(
    Guid Id,
    string EmployeeCode,
    string FirstName,
    string LastName,
    string Email,
    Guid UserId,
    Guid DepartmentId,
    string DepartmentName,
    DateTime CreatedAt);

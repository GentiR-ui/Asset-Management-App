using AssetManagementSystem.Domain.Enums;

namespace AssetManagementSystem.Domain.ReadModels;

/// <summary>
/// Forma e sheshte e nje aseti per LEXIM. Nuk eshte entitet: EF-i nuk e gjurmon kurre,
/// pra AsNoTracking nuk duhet, dhe nuk mund te shkruhet aksidentalisht ne databaze.
///
/// Ekziston qe query-ja te kerkoje VETEM kolonat e nevojshme. Me Include, nje faqe
/// asetesh terhiqte 49 kolona nga 4 tabela — perfshire PasswordHash dhe SecurityStamp —
/// per pak fusha qe perdoren vertet.
/// </summary>
public sealed record AssetListItem(
    Guid Id,
    string AssetTag,
    string Name,
    AssetCategory Category,
    string SerialNumber,
    AssetStatus Status,
    DateTime PurchaseDate,
    decimal PurchasePrice,
    DateTime? WarrantyExpiryDate,
    string? Notes,
    DateTime CreatedAt,

    // Të dhënat e Departamentit (me Description)
    Guid? DepartmentId,
    string? DepartmentName,
    string? DepartmentDescription,
    string? DepartmentCode,

    // Të dhënat e Punonjësit (të plota për EmployeeDto)
    Guid? EmployeeId,
    string? EmployeeCode,
    string? EmployeeFirstName,
    string? EmployeeLastName,
    string? EmployeeEmail);
using AssetManagementSystem.Domain.Enums;
namespace AssetManagementSystem.Domain.ReadModels;
/// <summary>
/// Forma e sheshte e nje aseti per LEXIM. Nuk eshte entitet: EF-i nuk e gjurmon kurre,
/// pra AsNoTracking nuk duhet, dhe nuk mund te shkruhet aksidentalisht ne databaze.
///
/// Ekziston qe query-ja te kerkoje VETEM kolonat e nevojshme. Me Include, nje faqe
/// asetesh terhiqte 49 kolona nga 4 tabela — perfshire PasswordHash dhe SecurityStamp —
/// per tre fusha qe perdoren vertet: emri i departamentit dhe emri/mbiemri i punonjesit.
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
    Guid DepartmentId,
    string DepartmentName,
    Guid? AssignedToEmployeeId,
    string? AssignedToEmployeeName);

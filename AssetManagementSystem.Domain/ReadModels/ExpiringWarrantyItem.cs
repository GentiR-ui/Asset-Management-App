namespace AssetManagementSystem.Domain.ReadModels;

/// <summary>
/// Vetem ato qe i duhen email-it te njoftimit: kater fusha nga 16 te tabeles Assets.
///
/// Me kete tip, DepartmentName nuk mund te jete null ne heshtje. Me entitetin dhe pa
/// Include, job-i lexonte asset.Department?.Name ?? "Pa departament" — dhe cdo rresht
/// i email-it thoshte "Pa departament", megjithese te dhenat ishin te sakta.
/// </summary>
public sealed record ExpiringWarrantyItem(
    string AssetTag,
    string Name,
    DateTime WarrantyExpiryDate,
    string DepartmentName);

using AssetManagementSystem.Application.DTOs.Dashboard;
using AssetManagementSystem.Domain.ReadModels;

namespace AssetManagementSystem.Application.Common.Mappings;

public static class DashboardMappings
{
    public static AssetValueSummaryResponse ToAssetValueSummaryResponse(this AssetValueSummary value) => new()
    {
        AssetCount = value.AssetCount,
        TotalValue = value.TotalValue
    };

    public static DepartmentValueResponse ToDepartmentValueResponse(this DepartmentAssetValue value) => new()
    {
        DepartmentId = value.DepartmentId,
        DepartmentName = value.DepartmentName,
        AssetCount = value.AssetCount,
        TotalValue = value.TotalValue
    };

    // Enum -> string ndodh ketu, jo ne SQL: SQL Server-i nuk i njeh emrat e enum-eve.
    public static StatusValueResponse ToStatusValueResponse(this StatusAssetValue value) => new()
    {
        Status = value.Status.ToString(),
        AssetCount = value.AssetCount,
        TotalValue = value.TotalValue
    };

    public static CategoryValueResponse ToCategoryValueResponse(this CategoryAssetValue value) => new()
    {
        Category = value.Category.ToString(),
        AssetCount = value.AssetCount,
        TotalValue = value.TotalValue
    };

    /// <summary>Etiketa vjen si parameter sepse varet nga kufijte, dhe kufijte i zgjedh sherbimi.</summary>
    public static AgeValueResponse ToAgeValueResponse(this AgeBucketCount value, string ageRange) => new()
    {
        AgeRange = ageRange,
        AssetCount = value.AssetCount,
        TotalValue = value.TotalValue
    };
}

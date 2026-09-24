using AssetManagementSystem.Domain.Enums;

namespace AssetManagementSystem.Domain.ReadModels;

/// <summary>null te nje filter do te thote "mos filtro sipas tij".</summary>
public sealed record AssetFilter(
    AssetCategory? Category,
    AssetStatus? Status,
    string? Search,
    int Page,
    int PageSize);

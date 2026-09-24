using AssetManagementSystem.Domain.Enums;

namespace AssetManagementSystem.Application.DTOs.Assets;

/// <summary>
/// ?category=Laptop&amp;status=InStock&amp;search=dell&amp;page=2&amp;pageSize=10
/// Fushat null do te thone "mos filtro sipas tyre".
/// </summary>
public sealed record AssetQueryRequest
{
    public AssetCategory? Category { get; init; }
    public AssetStatus? Status { get; init; }
    public string? Search { get; init; }

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

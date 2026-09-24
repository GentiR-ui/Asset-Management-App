namespace AssetManagementSystem.Application.DTOs.Common;

/// <summary>Parametrat e faqosjes per listat pa filtra. Lexohen nga URL-ja me [FromQuery].</summary>
public sealed record PageQueryRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

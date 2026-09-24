namespace AssetManagementSystem.Domain.ReadModels;

/// <summary>
/// Nje faqe rezultatesh plus totali i rreshtave qe e plotesojne filtrin.
/// Pa TotalCount, klienti s'di sa faqe ka.
/// </summary>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);

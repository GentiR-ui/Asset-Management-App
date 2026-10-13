namespace AssetManagementSystem.Domain.Common;
/// <summary>
/// Rezultati i gjenerimit te nje refresh token-i. <paramref name="Value"/> shkon te klienti
/// dhe nuk ruhet kurre; <paramref name="Hash"/> ruhet ne databaze.
/// </summary>
public sealed record RefreshTokenValue(string Value, string Hash, DateTime ExpiresAtUtc);

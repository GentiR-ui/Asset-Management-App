namespace AssetManagementSystem.Infrastructure.Identity;
public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;
    /// <summary>Jeta e access token-it. E shkurter me qellim: logout-i behet i ndjeshem
    /// vetem brenda kesaj dritareje, sepse access token-i nuk revokohet.</summary>
    public int ExpiryMinutes { get; init; } = 15;
    public int RefreshTokenDays { get; init; } = 7;
}

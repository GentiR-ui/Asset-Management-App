namespace AssetManagementSystem.Domain.Common;

public sealed record AccessToken(string Value,DateTime ExpiresAtUtc);

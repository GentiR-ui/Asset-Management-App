namespace AssetManagementSystem.Application.DTOs.Assets;

public sealed record AssignAssetRequest
{
    public Guid EmployeeId { get; init; }
}

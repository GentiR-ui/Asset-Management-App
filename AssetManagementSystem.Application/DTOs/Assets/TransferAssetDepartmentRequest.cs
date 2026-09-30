namespace AssetManagementSystem.Application.DTOs.Assets;

public record TransferAssetDepartmentRequest{
    public required Guid DepartmentId { get; init; }
}
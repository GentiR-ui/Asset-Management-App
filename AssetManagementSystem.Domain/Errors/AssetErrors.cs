using ErrorOr;

namespace AssetManagementSystem.Domain.Errors;

public static class AssetErrors
{
    public static Error NotFound(Guid assetId) => Error.NotFound(
        code: "Asset.NotFound",
        description: $"Asset with identifier '{assetId}' was not found.");

    public static Error AssetTagAlreadyExists(string assetTag) => Error.Conflict(
        code: "Asset.AssetTagAlreadyExists",
        description: $"An asset with tag '{assetTag}' already exists.");

    public static Error SerialNumberAlreadyExists(string serialNumber) => Error.Conflict(
        code: "Asset.SerialNumberAlreadyExists",
        description: $"An asset with serial number '{serialNumber}' already exists.");

   

}

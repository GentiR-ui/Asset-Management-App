namespace AssetManagementSystem.Application.Exceptions;

public sealed class NotFoundException : AppException
{
    public override AppErrorType ErrorType => AppErrorType.NotFound;

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with identifier '{key}' was not found.")
    {
    }
}

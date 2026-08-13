namespace AssetManagementSystem.Application.Exceptions;


public sealed class BadRequestException(string message = "The request is malformed or contains invalid data.") : AppException(message)
{
    public override AppErrorType ErrorType => AppErrorType.BadRequest;
}

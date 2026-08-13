namespace AssetManagementSystem.Application.Exceptions;

public sealed class UnauthorizedException(string message = "Authentication is required to access this resource.") : AppException(message)
{
    public override AppErrorType ErrorType => AppErrorType.Unauthorized;
}

namespace AssetManagementSystem.Application.Exceptions;

public abstract class AppException : Exception
{
    public abstract AppErrorType ErrorType { get; }

    protected AppException(string message)
        : base(message)
    {
    }

    protected AppException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
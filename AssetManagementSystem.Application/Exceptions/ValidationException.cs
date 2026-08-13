namespace AssetManagementSystem.Application.Exceptions;


public sealed class ValidationException : AppException
{
    public override AppErrorType ErrorType => AppErrorType.Validation;

    public IReadOnlyDictionary<string, string[]> Errors { get; }

    private const string DefaultMessage = "One or more validation errors occurred.";

    public ValidationException()
        : base(DefaultMessage)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base(DefaultMessage)
    {
        Errors = errors;
    }


    public ValidationException(string propertyName, string errorMessage)
        : base(DefaultMessage)
    {
        Errors = new Dictionary<string, string[]>
        {
            [propertyName] = [errorMessage]
        };
    }
}

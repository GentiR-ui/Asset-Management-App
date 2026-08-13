using AssetManagementSystem.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AssetManagementSystem.API.Filters;

public sealed class ExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionFilter> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionFilter(ILogger<ExceptionFilter> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        ProblemDetails problemDetails = context.Exception switch
        {
            
            ValidationException validationException => BuildValidationProblem(validationException),

            
            AppException appException => BuildAppProblem(appException),

            _ => BuildUnexpectedProblem(context.Exception)
        };

        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };

        
        context.ExceptionHandled = true;
    }

    private ValidationProblemDetails BuildValidationProblem(ValidationException exception)
    {
        _logger.LogWarning("Validation failed for {Path}", exception.Errors.Keys);

        return new ValidationProblemDetails(
            exception.Errors.ToDictionary(error => error.Key, error => error.Value))
        {
            Status = StatusCodes.Status400BadRequest,
            Title = exception.Message
        };
    }

    private ProblemDetails BuildAppProblem(AppException exception)
    {
        
        _logger.LogWarning(
            "Handled {ErrorType}: {Message}",
            exception.ErrorType,
            exception.Message);

        return new ProblemDetails
        {
            Status = ToStatusCode(exception.ErrorType),
            Title = exception.ErrorType.ToString(),
            Detail = exception.Message
        };
    }

    private ProblemDetails BuildUnexpectedProblem(Exception exception)
    {

        _logger.LogError(exception, "Unhandled exception");

        return new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",

            
            Detail = _environment.IsDevelopment() ? exception.ToString() : null
        };
    }


    private static int ToStatusCode(AppErrorType errorType) => errorType switch
    {
        AppErrorType.Validation => StatusCodes.Status400BadRequest,
        AppErrorType.BadRequest => StatusCodes.Status400BadRequest,
        AppErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        AppErrorType.Forbidden => StatusCodes.Status403Forbidden,
        AppErrorType.NotFound => StatusCodes.Status404NotFound,
        AppErrorType.Conflict => StatusCodes.Status409Conflict,
        AppErrorType.Unexpected => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };
}

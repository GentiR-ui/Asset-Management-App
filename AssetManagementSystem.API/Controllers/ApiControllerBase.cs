using System.Security.Claims;
using AssetManagementSystem.Application.Common.Responses;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementSystem.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
   
    protected Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    protected IActionResult Success<T>(T data, int statusCode = StatusCodes.Status200OK) =>
        new ObjectResult(new BaseResponse<T>
        {
            Success = true,
            StatusCode = statusCode,
            Data = data
        })
        { StatusCode = statusCode };

    protected IActionResult Success(string message, int statusCode = StatusCodes.Status200OK) =>
        new ObjectResult(new BaseResponse
        {
            Success = true,
            StatusCode = statusCode,
            Message = message
        })
        { StatusCode = statusCode };

    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
        {
            return BuildError(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            var response = new BaseResponse
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "One or more validation errors occurred.",
                Errors = errors
                    .GroupBy(error => error.Code)
                    .ToDictionary(group => group.Key, group => group.Select(e => e.Description).ToArray())
            };

            return new ObjectResult(response) { StatusCode = StatusCodes.Status400BadRequest };
        }

        var first = errors[0];
        return BuildError(ToStatusCode(first.Type), first.Description);
    }

    private static IActionResult BuildError(int statusCode, string message) =>
        new ObjectResult(new BaseResponse
        {
            Success = false,
            StatusCode = statusCode,
            Message = message
        })
        { StatusCode = statusCode };

    private static int ToStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation   => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden    => StatusCodes.Status403Forbidden,
        ErrorType.NotFound     => StatusCodes.Status404NotFound,
        ErrorType.Conflict     => StatusCodes.Status409Conflict,
        _                      => StatusCodes.Status500InternalServerError
    };
}

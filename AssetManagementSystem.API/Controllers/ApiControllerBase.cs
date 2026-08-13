using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AssetManagementSystem.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
            return Problem();

        if (errors.All(error => error.Type == ErrorType.Validation))
            return ValidationProblem(errors);        // ← (A)

        return Problem(errors[0]);                   // ← (B)
    }

    private IActionResult Problem(Error error) => Problem(
        statusCode: error.Type switch                // ← (C) përkthimi
        {
            ErrorType.Validation   => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden    => StatusCodes.Status403Forbidden,
            ErrorType.NotFound     => StatusCodes.Status404NotFound,
            ErrorType.Conflict     => StatusCodes.Status409Conflict,
            _                      => StatusCodes.Status500InternalServerError
        },
        title: error.Description,
        type: error.Code);

        private IActionResult ValidationProblem(List<Error> errors)
{
    var modelState = new ModelStateDictionary();

    foreach (var error in errors)
    {
        modelState.AddModelError(error.Code, error.Description);
    }

    return ValidationProblem(modelState);
}

}
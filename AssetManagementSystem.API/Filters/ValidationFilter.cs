using AssetManagementSystem.Application.Common.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AssetManagementSystem.API.Filters;

public sealed class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var modelState = new ModelStateDictionary();          // ← u fshi

        foreach (var argument in context.ActionArguments.Values)   // ← u fshi
        {
            if (argument is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

            if (_serviceProvider.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(
                validationContext, context.HttpContext.RequestAborted);

            foreach (var failure in result.Errors)
            {
                modelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
            }
        }

        if (!modelState.IsValid)
        {
            var response = new BaseResponse
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "One or more validation errors occurred.",
                Errors = modelState.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors.Select(e => e.ErrorMessage).ToArray())
            };

            context.Result = new ObjectResult(response) { StatusCode = StatusCodes.Status400BadRequest };
            return;
        }

        await next();                                          // ← u fshi
    }
}

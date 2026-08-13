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
        var modelState = new ModelStateDictionary();

        foreach (var argument in context.ActionArguments.Values)
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
            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            foreach (var failure in result.Errors)
            {
                modelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
            }
        }

        if (!modelState.IsValid)
        {
            // Ndalojmë këtu — controller-i nuk thirret fare.
            context.Result = new BadRequestObjectResult(new ValidationProblemDetails(modelState));
            return;
        }

        await next();
    }
}

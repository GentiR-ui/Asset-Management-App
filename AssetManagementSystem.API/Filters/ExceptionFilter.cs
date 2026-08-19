using AssetManagementSystem.Application.Common.Responses;
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
            var response = context.Exception switch
            {
                ValidationException e => new BaseResponse
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = e.Message,
                    Errors = e.Errors
                },

                BadRequestException e   => Build(StatusCodes.Status400BadRequest, e.Message),
                UnauthorizedException e => Build(StatusCodes.Status401Unauthorized, e.Message),
                ForbiddenException e    => Build(StatusCodes.Status403Forbidden, e.Message),
                NotFoundException e     => Build(StatusCodes.Status404NotFound, e.Message),
                ConflictException e     => Build(StatusCodes.Status409Conflict, e.Message),

                _ => BuildUnexpected(context.Exception)
            };

            context.Result = new ObjectResult(response) { StatusCode = response.StatusCode };
            context.ExceptionHandled = true;
        }

        private static BaseResponse Build(int statusCode, string message) => new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message
        };

        private BaseResponse BuildUnexpected(Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");

            return new BaseResponse
            {
                Success = false,
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = _environment.IsDevelopment()
                    ? exception.ToString()
                    : "An unexpected error occurred."
            };
        }

}

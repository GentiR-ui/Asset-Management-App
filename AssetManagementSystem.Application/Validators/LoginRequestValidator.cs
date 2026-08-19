using AssetManagementSystem.Application.DTOs.Auth;
using FluentValidation;

namespace AssetManagementSystem.Application.Validators;
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.Email).NotEmpty();
        RuleFor(request => request.Password).NotEmpty();
    }
}

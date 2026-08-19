using AssetManagementSystem.Application.DTOs.Auth;
using FluentValidation;

namespace AssetManagementSystem.Application.Validators;
public sealed class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(r => r.Email).NotEmpty().EmailAddress();
        RuleFor(r => r.Token).NotEmpty();
    }
}

public sealed class ResendConfirmationRequestValidator : AbstractValidator<ResendConfirmationRequest>
{
    public ResendConfirmationRequestValidator()
    {
        RuleFor(r => r.Email).NotEmpty().EmailAddress();
    }
}

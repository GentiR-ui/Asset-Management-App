using AssetManagementSystem.Application.DTOs.Auth;
using FluentValidation;

namespace AssetManagementSystem.Application.Validators;

public sealed class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(request => request.RefreshToken).NotEmpty();
    }
}

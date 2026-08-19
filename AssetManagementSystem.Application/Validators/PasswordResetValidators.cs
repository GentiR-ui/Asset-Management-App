using AssetManagementSystem.Application.DTOs.Auth;
using FluentValidation;

namespace AssetManagementSystem.Application.Validators;

public sealed class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(r => r.Email).NotEmpty().EmailAddress();
    }
}

public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(r => r.Email).NotEmpty();
        RuleFor(r => r.Token).NotEmpty();

        // Të njëjtat rregulla si te regjistrimi — një password i ri
        // duhet të jetë po aq i fortë sa i pari.
        RuleFor(r => r.NewPassword)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain a digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain a special character.");

        RuleFor(r => r.ConfirmNewPassword)
            .Equal(r => r.NewPassword)
            .WithMessage("Password and confirmation password do not match.");
    }
}

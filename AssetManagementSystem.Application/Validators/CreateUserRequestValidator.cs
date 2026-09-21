using AssetManagementSystem.Application.DTOs.Users;
using AssetManagementSystem.Domain.Common;
using FluentValidation;

namespace AssetManagementSystem.Application.Validators;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100);

        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100);

        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(request => request.Password)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain a digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain a special character.");

        // Kapet ketu, jo brenda transaksionit: nje rol i pavlefshem s'duhet te arrije kurre te Identity.
        RuleFor(request => request.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(role => AppRoles.All.Contains(role))
            .WithMessage($"Role must be one of: {string.Join(", ", AppRoles.All)}.");

        // Vetem stafi ka departament dhe kod punonjesi. Admin dhe Client jo.
        When(request => AppRoles.Staff.Contains(request.Role), () =>
        {
            RuleFor(request => request.EmployeeCode)
                .NotEmpty().WithMessage("Employee code is required for this role.")
                .MaximumLength(20);

            RuleFor(request => request.DepartmentId)
                .NotEmpty().WithMessage("Department is required for this role.");
        });    
    }
}

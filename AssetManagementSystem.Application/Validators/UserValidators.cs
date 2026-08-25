using AssetManagementSystem.Application.DTOs.Users;
using AssetManagementSystem.Domain.Common;
using FluentValidation;

namespace AssetManagementSystem.Application.Validators;

public sealed class AssignRoleRequestValidator : AbstractValidator<AssignRoleRequest>
{
    public AssignRoleRequestValidator()
    {
        RuleFor(request => request.RoleName)
            .NotEmpty().WithMessage("Role name is required.")
            .Must(role => AppRoles.All.Contains(role))
            .WithMessage($"Role must be one of: {string.Join(", ", AppRoles.All)}.");
    }
}

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");
    }
}

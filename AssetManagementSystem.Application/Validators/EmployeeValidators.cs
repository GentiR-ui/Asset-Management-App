using AssetManagementSystem.Application.DTOs.Employees;
using FluentValidation;

namespace AssetManagementSystem.Application.Validators;

public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.EmployeeCode)
            .NotEmpty().WithMessage("Employee code is required.")
            .MaximumLength(20).WithMessage("Employee code must not exceed 20 characters.");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Department id is required.");
    }
}

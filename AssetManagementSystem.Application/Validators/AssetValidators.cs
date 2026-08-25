using AssetManagementSystem.Application.DTOs.Assets;
using FluentValidation;

namespace AssetManagementSystem.Application.Validators;

public sealed class CreateAssetRequestValidator : AbstractValidator<CreateAssetRequest>
{
    public CreateAssetRequestValidator()
    {
        RuleFor(request => request.AssetTag)
            .NotEmpty().WithMessage("Asset tag is required.")
            .MaximumLength(50).WithMessage("Asset tag cannot exceed 50 characters.");

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("Asset name is required.")
            .MaximumLength(200).WithMessage("Asset name cannot exceed 200 characters.");

        // IsInEnum kap vlerat qe s'ekzistojne — p.sh. nese klienti dergon 99999.
        RuleFor(request => request.Category)
            .IsInEnum().WithMessage("Category is not a valid value.");

        RuleFor(request => request.SerialNumber)
            .NotEmpty().WithMessage("Serial number is required.")
            .MaximumLength(100).WithMessage("Serial number cannot exceed 100 characters.");

        RuleFor(request => request.PurchaseDate)
            .NotEmpty().WithMessage("Purchase date is required.")
            .LessThanOrEqualTo(_ => DateTime.UtcNow.Date.AddDays(1))
            .WithMessage("Purchase date cannot be in the future.");

        RuleFor(request => request.PurchasePrice)
            .GreaterThanOrEqualTo(0).WithMessage("Purchase price cannot be negative.");

        // Garancia duhet te jete PAS blerjes — perndryshe te dhena te pakuptimta.
        RuleFor(request => request.WarrantyExpiryDate)
            .GreaterThan(request => request.PurchaseDate)
            .When(request => request.WarrantyExpiryDate.HasValue)
            .WithMessage("Warranty expiry date must be after the purchase date.");

        RuleFor(request => request.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");
    }
}

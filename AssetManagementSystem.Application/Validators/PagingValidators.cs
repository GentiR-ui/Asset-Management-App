using AssetManagementSystem.Application.DTOs.Assets;
using AssetManagementSystem.Application.DTOs.Common;
using FluentValidation;

namespace AssetManagementSystem.Application.Validators;

public sealed class PageQueryRequestValidator : AbstractValidator<PageQueryRequest>
{
    public PageQueryRequestValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be 1 or greater.");

        // Kufiri i siperm nuk eshte dekor: pa te, ?pageSize=1000000 e kthen faqosjen ne GetAll.
        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}

public sealed class AssetQueryRequestValidator : AbstractValidator<AssetQueryRequest>
{
    public AssetQueryRequestValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be 1 or greater.");

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        RuleFor(request => request.Search)
            .MaximumLength(100).WithMessage("Search term cannot exceed 100 characters.");
    }
}

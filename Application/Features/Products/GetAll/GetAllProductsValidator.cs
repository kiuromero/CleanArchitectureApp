using Application.Features.Products.GetAll;
using FluentValidation;

public class GetAllProductsValidator : AbstractValidator<GetAllProductsQuery>
{
    private static readonly string[] AllowedFields =
    {
        "name",
        "price",
        "id"
    };

    public GetAllProductsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50);

        RuleFor(x => x.OrderBy)
            .Must(o => string.IsNullOrWhiteSpace(o) ||
                       AllowedFields.Contains(o, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Invalid orderBy field");
    }
}
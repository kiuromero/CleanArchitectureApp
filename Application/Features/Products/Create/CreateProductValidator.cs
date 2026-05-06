using FluentValidation;

namespace Application.Features.Products.Create;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Price)
            .GreaterThan(0);
    }
}
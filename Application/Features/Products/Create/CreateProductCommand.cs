using MediatR;

namespace Application.Features.Products.Create;

public class CreateProductCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
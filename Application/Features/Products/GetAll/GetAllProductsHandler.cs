using Application.Common.Models;
using Application.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Products.GetAll;

public class GetAllProductsHandler
    : IRequestHandler<GetAllProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _repository;

    public GetAllProductsHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ProductDto>> Handle(
    GetAllProductsQuery request,
    CancellationToken cancellationToken)
    {
        request.Page = Math.Max(request.Page, 1);
        request.PageSize = Math.Clamp(request.PageSize, 1, 50);

        var (items, totalCount) = await _repository.GetFilteredPagedAsync(
        request.Page,
        request.PageSize,
        request.Name,
        request.MinPrice,
        request.MaxPrice,
        request.OrderBy,
        request.Direction
        );

        var result = items.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
        }).ToList();

        return new PagedResult<ProductDto>
        {
            Items = result,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
using Application.Common.Enums;
using Application.Common.Models;
using Application.DTOs;
using MediatR;

public class GetAllProductsQuery : IRequest<PagedResult<ProductDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public string? OrderBy { get; set; } // campo
    public OrderDirection? Direction { get; set; } = OrderDirection.Asc;
}
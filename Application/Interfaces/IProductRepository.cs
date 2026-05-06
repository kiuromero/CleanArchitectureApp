using Application.Common.Enums;
using Domain.Entities;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task<(List<Product> Items, int TotalCount)> GetFilteredPagedAsync(
       int page,
       int pageSize,
       string? name,
       decimal? minPrice,
       decimal? maxPrice,
       string? orderBy,
       OrderDirection? direction
   );
}
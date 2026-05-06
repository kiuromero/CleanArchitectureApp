using Application.Common.Enums;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task<(List<Product> Items, int TotalCount)> GetFilteredPagedAsync(
    int page,
    int pageSize,
    string? name,
    decimal? minPrice,
    decimal? maxPrice,
    string? orderBy,
    OrderDirection? direction)
    {
        var query = _context.Products.AsQueryable();

        // filtros
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.Contains(name));

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice);

        // ordenamiento dinámico
        query = (orderBy?.ToLower(), direction) switch
        {
            ("price", OrderDirection.Desc) => query.OrderByDescending(p => p.Price),
            ("price", _) => query.OrderBy(p => p.Price),

            ("name", OrderDirection.Desc) => query.OrderByDescending(p => p.Name),
            ("name", _) => query.OrderBy(p => p.Name),

            ("id", OrderDirection.Desc) => query.OrderByDescending(p => p.Id),
            ("id", _) => query.OrderBy(p => p.Id),

            _ => query.OrderBy(p => p.Id)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
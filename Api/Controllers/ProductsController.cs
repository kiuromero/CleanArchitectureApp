using Application.Common.Models;
using Application.DTOs;
using Application.Features.Products.Create;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(ApiResponse<int>.Ok(id));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllProductsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<PagedResult<ProductDto>>.Ok(result));
    }
}
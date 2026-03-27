using CR.Common;
using CR.Core.ApplicationServices.ProductModule.Abstracts;
using CR.Core.Dtos.Product;
using CR.WebAPIBase.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CR.Core.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResult<ProductResponseDto>>>> GetAll(
        [FromQuery] int page = 1, 
        [FromQuery] int size = 10)
    {
        var result = await _productService.GetAllAsync(page, size);
         return Ok(ApiResponse<PaginatedResult<ProductResponseDto>>.Ok(result));
    }
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Create([FromBody] ProductRequestDto dto)
    {
        var result = await _productService.CreateAsync(dto);
        return Ok(ApiResponse<ProductResponseDto>.Ok(result));
    }
}
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
        [FromQuery] int size = 10,
        [FromQuery] int? categoryId = null)
    {
        var result = await _productService.GetAllAsync(page, size, categoryId);
        return Ok(ApiResponse<PaginatedResult<ProductResponseDto>>.Ok(result));
    }
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Create([FromBody] ProductRequestDto dto)
    {
        var result = await _productService.CreateAsync(dto);
        return Ok(ApiResponse<ProductResponseDto>.Ok(result));
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        if (result.IsFailure)
            return BadRequest(result);

        return Ok(ApiResponse<ProductResponseDto>.Ok(result.Value));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Update(int id, [FromBody] ProductRequestDto dto)
    {
        var result = await _productService.UpdateAsync(id, dto);
        if (result.IsFailure)
            return BadRequest(result);

        return Ok(ApiResponse<ProductResponseDto>.Ok(result.Value));
    }
    [HttpPost("variants")]
    public async Task<ActionResult<ApiResponse<ProductVariantResponseDto>>> CreateVariant([FromBody] CreateProductVariantDto dto)
    {
        var result = await _productService.CreateProductVariantAsync(dto);
        if (result.IsFailure)
            return BadRequest(result);

        return Ok(ApiResponse<ProductVariantResponseDto>.Ok(result.Value));
    }

    [HttpPut("variants/{variantId}")]
    public async Task<ActionResult<ApiResponse<ProductVariantResponseDto>>> UpdateVariant(int variantId, [FromBody] UpdateProductVariantDto dto)
    {
        var result = await _productService.UpdateProductVariantAsync(variantId, dto);
        if (result.IsFailure)
            return BadRequest(result);

        return Ok(ApiResponse<ProductVariantResponseDto>.Ok(result.Value));
    }
}
using CR.Common;
using CR.Core.ApplicationServices.ProductModule.Abstracts;
using CR.Core.Dtos.Product;
using CR.WebAPIBase.Responses;
using Microsoft.AspNetCore.Mvc;
using CR.DtoBase;
using Microsoft.AspNetCore.Http;
using CR.Core.API.Extensions;

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
    [ProducesResponseType(typeof(ApiResponse<PageResult<ProductResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] ProductQueryDto query)
        => (await _productService.GetAllAsync(query)).ToActionResult(this);

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ProductRequestDto dto)
        => (await _productService.CreateAsync(dto)).ToActionResult(this, "Tạo sản phẩm thành công");

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
        => (await _productService.GetByIdAsync(id)).ToActionResult(this);

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] ProductRequestDto dto)
        => (await _productService.UpdateAsync(id, dto)).ToActionResult(this, "Cập nhật sản phẩm thành công");

    [HttpPost("variants")]
    [ProducesResponseType(typeof(ApiResponse<ProductVariantResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateVariant([FromBody] CreateProductVariantDto dto)
        => (await _productService.CreateProductVariantAsync(dto)).ToActionResult(this, "Tạo biến thể thành công");

    [HttpPut("variants/{variantId}")]
    [ProducesResponseType(typeof(ApiResponse<ProductVariantResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVariant(int variantId, [FromBody] UpdateProductVariantDto dto)
        => (await _productService.UpdateProductVariantAsync(variantId, dto)).ToActionResult(this, "Cập nhật biến thể thành công");

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
        => (await _productService.DeleteAsync(id)).ToActionResult(this, "Xóa sản phẩm thành công");
}
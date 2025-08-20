using E_Commerce.Business.ViewModels.Dtos;
using E_Commerce.Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using E_Commerce.Business.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using E_Commerce.DataAccess.Constants;

namespace E_Commerce.Web.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _productService.GetAllProductsDtoAsync();
            if (products == null || !products.Any())
                return NotFound("No products found.");
            return Ok(products);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> Get(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid product ID.");
            var product = await _productService.GetProductDto(id);
            if (product == null)
                return NotFound("Product not found.");
            return Ok(product);
        }

        [HttpPost("Add")]
        [Authorize(Roles = AppRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> AddProduct([FromForm] ProductPostDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var created = await _productService.AddProductAsync(dto);
            return Created();
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] ProductUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");
            var updated = await _productService.UpdateProductAsync(dto);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0) return BadRequest();
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}

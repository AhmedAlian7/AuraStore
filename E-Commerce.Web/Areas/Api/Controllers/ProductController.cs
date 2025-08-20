using E_Commerce.Business.ViewModels.Dtos;
using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using E_Commerce.Business.Services.Implementation;

namespace E_Commerce.Web.Areas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitIfWork;
        public ProductController(IProductService productService, IUnitOfWork unitOfWork)
        {
            _productService = productService;
            _unitIfWork = unitOfWork;
        }

        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _productService.GetAllProductsDtoAsync();
            if (products == null || !products.Any())
                return NotFound("No products found.");
            return Ok(products);
        }

        [HttpGet("{id}")]
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
        public async Task<ActionResult<ProductDto>> AddProduct([FromForm] ProductPostDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var created = await _productService.AddProductAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("Update/{id}")]
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
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}

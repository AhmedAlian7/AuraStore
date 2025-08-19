using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Product;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsDtoAsync();

            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }

            return Ok(products);
        }


        [HttpGet("{Id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll(int Id)
        {

            if (Id <= 0)
            {
                return BadRequest("Invalid product ID.");
            }

            var product = await _productService.GetProductDto(Id);

            if (product == null)
            {
                return NotFound("No products found.");
            }

            return Ok(product);
        }

        [HttpPost("Add")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddProduct([FromBody] ProductAddDto product)
        {
            if (product == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid product data.");
            }
            await _productService.AddProductAsync(product);
            return CreatedAtAction(nameof(GetAll), new { Id = product.Id }, product);
        }
}

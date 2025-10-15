using BLL.IService;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PRM_BE.DTO;

namespace PRM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDTO product)
        {
            if (product == null)
            {
                return BadRequest("Product is null");
            }
            var newProduct = new Product
            {
                ProductName = product.ProductName,
                BriefDescription = product.BriefDescription,
                FullDescription = product.FullDescription,
                TechnicalSpecifications = product.TechnicalSpecifications,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId

            };
            await _productService.AddProductAsync(newProduct);
            return Ok(new { message = "Product created successfully" });

        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct(int productId,[FromBody] ProductDTO product)
        {
          
            var existingProduct = await _productService.GetProductByIdAsync(productId);
            if (existingProduct == null)
            {
                return NotFound("Product not found");
            }
            existingProduct.ProductName = product.ProductName;
            existingProduct.BriefDescription = product.BriefDescription;
            existingProduct.FullDescription = product.FullDescription;
            existingProduct.TechnicalSpecifications = product.TechnicalSpecifications;
            existingProduct.Price = product.Price;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.CategoryId = product.CategoryId;
            await _productService.UpdateProductAsync(existingProduct);
            return Ok(new { message ="Product updated"});
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var existingProduct = await _productService.GetProductByIdAsync(productId);
            if (existingProduct == null)
            {
                return NotFound("Product not found");
            }
            await _productService.DeleteProductAsync(productId);
            return Ok(new { message = "Product deleted" });
        }
    }
}

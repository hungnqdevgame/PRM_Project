using BLL.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRM_BE.DTO;

namespace PRM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<CartDTO>> GetCartAsync(int userId)
        {
            var cart = await _cartService.GetCartAsync(userId);
            if (cart == null) return null;

            return new CartDTO
            {
                CartId = cart.CartId,
                UserId = cart.UserId ?? 0,
                TotalPrice = cart.CartItems.Sum(ci => ci.Price * ci.Quantity),
                Status = cart.Status,
                Items = cart.CartItems.Select(ci => new CartItemDTO
                {
                    CartItemId = ci.CartItemId,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.ProductName,
                    Quantity = ci.Quantity,
                    Price = ci.Price
                }).ToList()
            };
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO request)
        {
            await _cartService.AddToCartAsync(request.UserId, request.ProductId, request.Quantity);
            await _cartService.SaveChangesAsync();
            return Ok(new { message = "Product added to cart" });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCart([FromBody] AddToCartDTO request)
        {
            try
            {
                await _cartService.UpdateCartAsync(request.UserId,request.ProductId ,request.Quantity);
                return Ok(new { message = "Cart updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("increase")]
        public async Task<IActionResult> Increase([FromBody] UpdateCartDTO request)
        {
            await _cartService.UpdateCartItemAsync(request.UserId, request.ProductId, 1);
            return Ok("Increased successfully");
        }

        [HttpPost("decrease")]
        public async Task<IActionResult> Decrease([FromBody] UpdateCartDTO request)
        {
            await _cartService.UpdateCartItemAsync(request.UserId, request.ProductId, -1);
            return Ok("Decreased successfully");
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] UpdateCartDTO request)
        {
            await _cartService.RemoveCartItemAsync(request.UserId, request.ProductId);
            return Ok("Remove successfully");
        }



    }
}

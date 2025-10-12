using BLL.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRM_BE.DTO;
using System.Security.Claims;
namespace PRM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet()]
        public async Task<ActionResult<CartDTO>> GetCartAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            await _cartService.AddToCartAsync(userId, request.ProductId, request.Quantity);
            await _cartService.SaveChangesAsync();
            return Ok(new { message = "Product added to cart" });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCart([FromBody] AddToCartDTO request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            try
            {
                await _cartService.UpdateCartAsync(userId,request.ProductId ,request.Quantity);
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            await _cartService.UpdateCartItemAsync(userId, request.ProductId, 1);
            return Ok("Increased successfully");
        }

        [HttpPost("decrease")]
        public async Task<IActionResult> Decrease([FromBody] UpdateCartDTO request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            await _cartService.UpdateCartItemAsync(userId, request.ProductId, -1);
            return Ok("Decreased successfully");
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] UpdateCartDTO request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            await _cartService.RemoveCartItemAsync(userId, request.ProductId);
            return Ok("Remove successfully");
        }



    }
}

using BLL.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRM_BE.DTO;

namespace PRM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAll();
            return Ok(orders);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDTO dto)
        {
            var order = await _orderService.CreateOrder(dto.UserId, dto.PaymentMethod, dto.Address);
            return Ok(order);
        }
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateOrder(UpdateOrderDTO dto)
        {
            await _orderService.UpdateOrder(dto.OrderId, dto.OrderStatus);
            return Ok();
        }


        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            if (order == null)
                return NotFound();

            // Lấy cart có sản phẩm
            var cart = order.Cart;
            var cartItems = cart?.CartItems.Select(ci => new
            {
                ci.ProductId,
                ProductName = ci.Product?.ProductName,
                ci.Quantity,
                ci.Price
            }).ToList();

            return Ok(new
            {
                order.OrderId,
                order.PaymentMethod,
                order.OrderStatus,
                order.OrderDate,
                Products = cartItems
            });
        }
    }
}

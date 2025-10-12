using BLL.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> CreateOrder(int userId, string paymentMethod, string address)
        {
            var order = await _orderService.CreateOrder(userId, paymentMethod, address);
            return Ok(order);
        }
        [HttpPut("{userOrderId}")]
        public async Task<IActionResult> UpdateOrder(int userOrderId, string status)
        {
            await _orderService.UpdateOrder(userOrderId, status);
            return NoContent();
        }
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
    }
}

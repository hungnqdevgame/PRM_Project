using BLL.IService;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using PRM_BE.DTO;



namespace PRM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly PayOS _payOS;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;


        public CheckoutController(PayOS payOS, IHttpContextAccessor httpContextAccessor, IUserService userService, IOrderService orderService, ICartService cartService)
        {
            _payOS = payOS;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _orderService = orderService;
            _cartService = cartService;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            // Trả về trang HTML có tên "MyView.cshtml"
            return Redirect("index");
        }
        [HttpGet("/cancel")]
        public IActionResult Cancel()
        {
            // Trả về trang HTML có tên "MyView.cshtml"
            return Redirect("cancel");
        }
        [HttpGet("check-status")]
        public async Task<ActionResult<string>> Success()
        {
           
              
                var session = _httpContextAccessor.HttpContext.Session;
                var orderCodeStr = session.GetString("lastOrderCode");

                if (string.IsNullOrEmpty(orderCodeStr))
                {
                    Console.WriteLine("⚠️ Không tìm thấy orderCode trong Session!");
                    return "Khong co don hang";
                }

                long orderCode = long.Parse(orderCodeStr);
                Console.WriteLine($"🔹 /success được gọi với orderCode: {orderCode}");

                // 2️⃣ Gọi PayOS để lấy thông tin thanh toán
                var paymentInfo = await _payOS.getPaymentLinkInformation(orderCode);
                Console.WriteLine($"🔹 Trạng thái thanh toán từ PayOS: {paymentInfo.status}");

            
            return paymentInfo.status;
      }

        [HttpPost("create-payment-link")]
        public async Task<IActionResult> Checkout(CheckoutDTO dto, [FromServices] SalesAppDBContext context)
        {
            try
            {
                // 🔹 Tạo mã order duy nhất
                long orderCode = long.Parse(DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
                var cart = await _orderService.GetCartByOrderId(dto.OrderId);
                var cartItems = cart.CartItems.Select(ci => new
                {
                    ProductName = ci.Product.ProductName,
                    Quantity = ci.Quantity,
                    Price = ci.Price
                }).ToList();
                var items = cartItems.Select(ci => new ItemData(
                                                 ci.ProductName,
                                                 ci.Quantity,
                                                 Convert.ToInt32(ci.Price)
                                             )).ToList();
                //foreach (var cartItem in cart.CartItems)
                //{
                //    var item = new ItemData(cartItem.Product.ProductName, cartItem.Quantity,int.Parse(cartItem.Price.ToString() ));
                //    items.Add(item);
                //}
                    
         
                var order = await _orderService.GetOrderByIdAsync(dto.OrderId);
                _httpContextAccessor.HttpContext.Session.SetString("lastOrderCode", orderCode.ToString());
                var request = _httpContextAccessor.HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";

                // 🔹 Tạo link thanh toán
                var paymentData = new PaymentData(
                    orderCode,
                    Convert.ToInt32(order.Cart.TotalPrice),
                    "Thanh toán gói dịch vụ",
                    items,
                    $"{baseUrl}/cancel",
                    $"{baseUrl}/success"
                );

                var createPayment = await _payOS.createPaymentLink(paymentData);

                
                var payment = new Payment
                {
                    Amount = cart.TotalPrice,
                    OrderId = dto.OrderId,
                    PaymentStatus = "Pending",
                    PaymentDate = DateTime.UtcNow
                };

                context.Payments.Add(payment);
                await context.SaveChangesAsync();

                Console.WriteLine($" Đã lưu Payment Order {orderCode} cho User ");

            
                return Ok(new { checkoutUrl = createPayment.checkoutUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Checkout error: {ex.Message}");
                return BadRequest(new { message = "Payment creation failed" });
            }
        }


    }

}

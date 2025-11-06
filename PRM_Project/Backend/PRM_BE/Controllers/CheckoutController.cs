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
        [HttpGet("success")]
        public async Task<IActionResult> Success()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var orderCodeStr = session.GetString("lastOrderCode");
            var orderId = session.GetString("orderId");

            if (string.IsNullOrEmpty(orderCodeStr))
            {
                Console.WriteLine("⚠️ Không tìm thấy orderCode trong Session!");
                return StatusCode(500, new
                {
                    status = "FAILED",
                    message = "Không tìm thấy mã đơn hàng trong session."
                });
            }

            try
            {
                long orderCode = long.Parse(orderCodeStr);
                Console.WriteLine($"🔹 /success được gọi với orderCode: {orderCode}");

                var paymentInfo = await _payOS.getPaymentLinkInformation(orderCode);
                Console.WriteLine($"🔹 Trạng thái thanh toán từ PayOS: {paymentInfo.status}");

                // Nếu trạng thái thanh toán là "PAID" hoặc "SUCCEEDED"
                if (paymentInfo.status == "PAID" || paymentInfo.status == "SUCCEEDED")
                {
                    _orderService.UpdateOrder(int.Parse(orderId),"Completed");
                    return Ok(new
                    {
                        status = "SUCCESS",
                        message = "Thanh toán thành công",
                        data = paymentInfo
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        status = "FAILED",
                        message = $"Thanh toán chưa hoàn tất. Trạng thái hiện tại: {paymentInfo.status}"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi gọi PayOS: {ex.Message}");
                return StatusCode(500, new
                {
                    status = "FAILED",
                    message = "Lỗi hệ thống khi lấy thông tin thanh toán.",
                    error = ex.Message
                });
            }
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
                _httpContextAccessor.HttpContext.Session.SetString("orderId", dto.OrderId.ToString());
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

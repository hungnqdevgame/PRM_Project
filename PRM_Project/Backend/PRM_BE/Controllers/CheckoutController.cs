using BLL.IService;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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


        public CheckoutController(PayOS payOS, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _payOS = payOS;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
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
        //[HttpGet("/success")]
        //public async Task<IActionResult> Success([FromQuery] long orderCode, [FromServices] SalesAppDBContext context)
        //{
        //    try
        //    {
        //        Console.WriteLine($"🔹 /success called with orderCode: {orderCode}");

        //        // 🔹 Lấy thông tin thanh toán từ PayOS
        //        var paymentInfo = await _payOS.getPaymentLinkInformation(orderCode);
        //        Console.WriteLine($"🔹 paymentInfo.status: {paymentInfo.status}");
        //        Console.WriteLine($"🔹 paymentInfo.amount: {paymentInfo.amount}");


        //        // 🔹 Lấy Payment trong DB
        //        var payment = await context.Payments.FirstOrDefaultAsync(p => p.OrderCode == orderCode);
        //        if (payment == null)
        //        {
        //            Console.WriteLine($"⚠️ Payment với OrderCode {orderCode} không tìm thấy trong DB!");
        //        }
        //        else if (payment.IsSuccess)
        //        {
        //            Console.WriteLine($"ℹ️ Payment đã được đánh dấu thành công trước đó.");
        //        }
        //        else
        //        {
        //            Console.WriteLine($"💾 Payment tìm thấy: UserId={payment.UserId}, SubscriptionId={payment.SubscriptionId}, Amount={payment.Amount}");
        //            payment.IsSuccess = true;
        //            await context.SaveChangesAsync();
        //            Console.WriteLine($"✅ Payment OrderCode {orderCode} đã đánh dấu thành công.");

        //            // 🔹 Cập nhật subscription
        //            await _userService.UpdateSupscriptionStatus(payment.UserId, payment.SubscriptionId);
        //            Console.WriteLine($"✅ Subscription của User {payment.UserId} đã cập nhật thành SubscriptionId {payment.SubscriptionId}");
        //        }

        //        return Redirect("/success"); // Trang Blazor
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"❌ Exception in /success: {ex}");
        //        return Redirect("/success");
        //    }
        //}

        [HttpPost("/create-payment-link")]
        public async Task<IActionResult> Checkout(CheckoutDTO dto, [FromServices] SalesAppDBContext context)
        {
            try
            {
                // 🔹 Tạo mã order duy nhất
                long orderCode = long.Parse(DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
                var item = new ItemData(dto.SupscriptionName, 1, dto.Amount);
                var items = new List<ItemData> { item };

                // 🔹 Lấy base URL (ví dụ https://localhost:7102)
                var request = _httpContextAccessor.HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";

                // 🔹 Tạo link thanh toán
                var paymentData = new PaymentData(
                    orderCode,
                    dto.Amount,
                    "Thanh toán gói dịch vụ",
                    items,
                    $"{baseUrl}/cancel",
                    $"{baseUrl}/success"
                );

                var createPayment = await _payOS.createPaymentLink(paymentData);

                // 🔹 Lưu thông tin Payment vào DB
                //var payment = new Payment
                //{
                //    UserId = dto.UserId,
                //    SubscriptionId = dto.SubscriptionId,
                //    Amount = dto.Amount,
                //    OrderCode = orderCode,
                //    IsSuccess = false,
                //    CreateAt = DateTime.UtcNow
                //};

                //context.Payments.Add(payment);
                await context.SaveChangesAsync();

                Console.WriteLine($"💾 Đã lưu Payment Order {orderCode} cho User ");

                // 🔹 Trả URL cho Blazor client
                return Ok(new { checkoutUrl = createPayment.checkoutUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Checkout error: {ex.Message}");
                return BadRequest(new { message = "Payment creation failed" });
            }
        }


    }

}

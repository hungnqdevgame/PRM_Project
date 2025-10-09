using Azure.Core;
using BLL.IService;
using DAL.Model.Momo;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRM_BE.DTO;

namespace PRM_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private IMomoService _momoService;
        private ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly SalesAppDBContext _context ;
        //  private readonly IVnPayService _vnPayService;
        public PaymentController(IMomoService momoService,ICartService cartService, IOrderService orderService, SalesAppDBContext context)
        {
            _momoService = momoService;
            _cartService = cartService;
            _orderService = orderService;
            _context = context;
        }
        [HttpPost("CreatePaymentUrl")]
        public async Task<IActionResult> CreatePaymentMomo(int orderId)
        {
            var totalAmount = await _orderService.GetTotalAmount(orderId);
            if (totalAmount <= 0)
                return BadRequest("Tổng tiền không hợp lệ.");

            // 2️⃣ Tạo payment entity
            var payment = new Payment
            {
                OrderId = orderId,
                Amount = totalAmount,
                PaymentDate = DateTime.UtcNow,
                PaymentStatus = "Pending"
            };

            // 3️⃣ Gọi service MoMo
            var momoResponse = await _momoService.CreatePaymentAsync(payment);

            // 4️⃣ Trả về payUrl cho FE redirect
            if (momoResponse?.PayUrl == null)
                return BadRequest("Không thể tạo liên kết thanh toán MoMo.");

            return Ok(new { payUrl = momoResponse.PayUrl });
        }
        

        [HttpGet("PaymentCallBack")]
        public IActionResult PaymentCallback()
        {
            var response = _momoService.PaymentExcuteAsync(Request.Query);

            return Ok(response);
        }

        [HttpPost("MomoNotify")]
        public IActionResult MomoNotify()
        {
            var response = _momoService.PaymentExcuteAsync(Request.Query);
            // TODO: xử lý cập nhật DB
            return Ok(response);
        }

      

    }
}

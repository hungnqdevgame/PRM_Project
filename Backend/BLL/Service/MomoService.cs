using BLL.IService;
using DAL.IRepository;
using DAL.Model;
using DAL.Model.Momo;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOption> _options;
        private readonly IPaymentRepository _paymentRepository;
        public MomoService(IOptions<MomoOption> options, IPaymentRepository paymentRepository)
        {
            _options = options;
            _paymentRepository = paymentRepository;
        }

        public async Task<MomoCreatePaymentResponse> CreatePaymentAsync(Payment payment)
        {
            payment.PaymentId = DateTime.UtcNow.Ticks;
            payment.PaymentStatus = "false";
            payment.PaymentDate = DateTime.UtcNow;

            await _paymentRepository.AddAsync(payment);

            var orderInfo = $"Thanh toán đơn hàng #{payment.OrderId} cho user {payment.User.}";

            var rawData =
                $"partnerCode={_options.Value.PartnerCode}" +
                $"&accessKey={_options.Value.AccessKey}" +
                $"&requestId={payment.OrderId}" +
                $"&amount={payment.Amount}" +
                $"&orderId={payment.OrderId}" +
                $"&orderInfo={orderInfo}" +
                $"&returnUrl={_options.Value.ReturnUrl}" +
                $"&notifyUrl={_options.Value.NotifyUrl}" +
                $"&extraData=";


            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            var client = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");


            var requestData = new
            {
                accessKey = _options.Value.AccessKey,
                partnerCode = _options.Value.PartnerCode,
                requestType = _options.Value.RequestType,
                notifyUrl = _options.Value.NotifyUrl,
                returnUrl = _options.Value.ReturnUrl,
                orderId = payment.OrderId,
                amount = payment.Amount.ToString(),
                orderInfo = orderInfo,
                requestId = payment.OrderId,
                extraData = "",
                signature = signature
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);
            var momoResponse = JsonConvert.DeserializeObject<MomoCreatePaymentResponse>(response.Content);
            return momoResponse;

        }

        public async Task<MomoExecuteResponse> PaymentExcuteAsync(IQueryCollection collection)
        {
            var amount = collection.First(s => s.Key == "amount").Value;
            var orderInfo = collection.First(s => s.Key == "orderInfo").Value;
            var orderId = collection.First(s => s.Key == "orderId").Value;
            var resultCode = collection.First(s => s.Key == "resultCode").Value;

            var payment = await _paymentRepository.GetByIdAsync(orderId);
            if (payment != null)
            {
                payment.PaymentStatus = resultCode == true; // 0 = thành công
                await _paymentRepository.UpdateAsync(payment);
            }
            return new MomoExecuteResponse()
            {
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo

            };

        }
        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }

    }

}

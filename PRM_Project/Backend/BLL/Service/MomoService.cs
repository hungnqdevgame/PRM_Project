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
           payment.PaymentStatus = "Pending";
            payment.PaymentDate = DateTime.UtcNow;
            await _paymentRepository.AddAsync(payment);
            long amount = (long)Math.Round(payment.Amount, 0, MidpointRounding.AwayFromZero);

            var orderInfo = $"Thanh toán  hàng #{payment.OrderId}";

            var rawData =
                $"partnerCode={_options.Value.PartnerCode}" +
                $"&accessKey={_options.Value.AccessKey}" +
                $"&requestId={payment.PaymentId}" +
                $"&amount={amount}" +
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
                orderId = payment.OrderId.ToString(),
                amount = amount,
                orderInfo = orderInfo,
                requestId = payment.PaymentId.ToString(),
                extraData = "",
                signature = signature
            };
            Console.WriteLine($"[MoMo] Amount gửi đi: {amount} ({amount.GetType()})");
            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);
            Console.WriteLine(JsonConvert.SerializeObject(requestData, Formatting.Indented));
            var response = await client.ExecuteAsync(request);

            if (response == null || string.IsNullOrEmpty(response.Content))
                throw new Exception("Không nhận được phản hồi từ MoMo API");

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
                payment.PaymentStatus =  "0"; // 0 = thành công
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

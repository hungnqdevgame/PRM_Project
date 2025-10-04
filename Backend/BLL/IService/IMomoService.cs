using DAL.Model.Momo;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IService
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponse> CreatePaymentAsync(Order order);
        Task<MomoExecuteResponse> PaymentExcuteAsync(IQueryCollection request);
    }
}

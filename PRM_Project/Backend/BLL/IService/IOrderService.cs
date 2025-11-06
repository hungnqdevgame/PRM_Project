using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IService
{
    public interface IOrderService
    {
        Task<List<Order>> GetAll();
        Task<Order> CreateOrder(int userId, string paymentMethod, string address);
        Task<Order> UpdateOrder(int orderId, string status);

        Task AddAsync(Order order);

        Task<Order> GetOrderByIdAsync(int orderId);
        Task AddOrder(Order order);

        Task<decimal> GetTotalAmount(int orderId);

        Task<Cart> GetCartByOrderId(int orderId);
        Task<Order?> GetByIdAsync(int orderId);
    }
}

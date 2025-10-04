using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAll();
        Task<Order> CreateOrder(int userId, string paymentMethod, string address);
        Task UpdateOrder(int userOrderId, string status);

        Task AddAsync(Order order);

        Task<Order> GetOrderByIdAsync(int orderId);
    }
}

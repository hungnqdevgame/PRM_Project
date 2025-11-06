using BLL.IService;
using DAL.IRepository;
using DAL.Models;

using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BLL.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        public OrderService(IOrderRepository orderRepository,ICartRepository cartRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
        }

        public Task AddAsync(Order order)
      => _orderRepository.AddAsync(order);

        public Task AddOrder(Order order)
       => _orderRepository.AddOrder(order);

        public Task<Order> CreateOrder(int userId, string paymentMethod, string address)
      => _orderRepository.CreateOrder(userId, paymentMethod, address);

        public Task<List<Order>> GetAll()
      => _orderRepository.GetAll();

        public Task<Cart> GetCartByOrderId(int orderId)
       => _orderRepository.GetCartByOrderId(orderId);

        public Task<Order> GetOrderByIdAsync(int orderId)
       => _orderRepository.GetOrderByIdAsync(orderId);

        public Task<decimal> GetTotalAmount(int orderId)
      => _orderRepository.GetTotalAmount(orderId);

        public Task<Order> UpdateOrder(int orderId, string status)
      => _orderRepository.UpdateOrder(orderId, status);

        public async Task<Order?> GetByOrderIdAsync(int orderId)
       =>await _orderRepository.GetByIdAsync(orderId);

        public async Task<Order?> GetByIdAsync(int orderId)
       =>await _orderRepository.GetByIdAsync(orderId);
    }
}

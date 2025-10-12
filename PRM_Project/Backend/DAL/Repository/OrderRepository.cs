using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SalesAppDBContext _context;
        public OrderRepository(SalesAppDBContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAll()
        {
            var order = new List<Order>();
            return order = await _context.Orders.ToListAsync();
        }

        public async Task<Order> CreateOrder(int userId, string paymentMethod, string address)
        {
            var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId);
            
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                throw new Exception("Cart not found or is empty");
            }

            var order = new Order
            {
                UserId = userId,
                CartId = cart.CartId,
                OrderDate = DateTime.Now,
                BillingAddress = address,
                PaymentMethod = paymentMethod,
                OrderStatus = "Pending",
                    
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task UpdateOrder(int userOrderId, string status)
        {
            var order = await _context.Orders.FindAsync(userOrderId);
            if(order == null)
            {
                throw new Exception("not found");
            }
            order.OrderStatus = status;
            if(status == "Completed")
            {
               order.OrderStatus = "Success";

            }
            else
            {
                order.OrderStatus = "Fail";
            }
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders.FindAsync(orderId);
        }

        public async Task AddOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalAmount(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Cart)
                .ThenInclude(c => c.CartItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null || order.Cart == null || order.Cart.CartItems == null)
            {
                throw new Exception("Order or Cart not found");
            }
           var amount = order.Cart.TotalPrice;
           
         return  amount;
        }
    }
}

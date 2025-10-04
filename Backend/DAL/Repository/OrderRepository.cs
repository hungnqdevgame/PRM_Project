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

        public async Task<Order> CreateOrder(int userId,string paymentMethod,string address)
        {
            var cart = await _context.Carts.FindAsync(userId);
            
            if(cart == null)
            {
                 throw new Exception("not found");
            } 

            var order  = await _context.Orders.Include(c =>c.Cart)
                .FirstOrDefaultAsync(u =>u.UserId == userId && u.CartId == cart.CartId);

            if(order != null)
            {
                var newOrder = new Order
                {
                    UserId = userId,
                    CartId = cart.CartId,                
                    OrderDate = DateTime.Now,
                    BillingAddress = address,
                    PaymentMethod = paymentMethod,
                    OrderStatus = "Pending"
                };
            }
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
    }
}

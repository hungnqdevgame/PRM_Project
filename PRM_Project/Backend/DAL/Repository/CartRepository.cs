using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace DAL.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly SalesAppDBContext _context;
        public CartRepository(SalesAppDBContext context)
        {
            _context = context;
        }
        public async Task<Cart?> GetCartAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new Exception("Product not found");

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Status = "Active",
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingItem == null)
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price
                });
            }
            else
            {
                existingItem.Quantity += quantity;
            }

            cart.TotalPrice = cart.CartItems.Sum(ci => ci.Price * ci.Quantity);

        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartAsync(int userId, int productId, int quantity)
        {
            var cart = await _context.Carts
        .Include(c => c.CartItems)
        .ThenInclude(ci => ci.Product)
        .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                throw new Exception("Cart not found");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (quantity <= 0)
            {
                // Xóa sản phẩm khỏi giỏ hàng
                if (cartItem != null)
                {
                    _context.CartItems.Remove(cartItem);
                }
            }
            else
            {
                if (cartItem != null)
                {
                    // Cập nhật số lượng
                    cartItem.Quantity = quantity;
                    cartItem.Price = cartItem.Product.Price * quantity;
                }
                else
                {
                    // Thêm mới nếu chưa có trong giỏ hàng
                    var product = await _context.Products.FindAsync(productId);
                    if (product == null)
                        throw new Exception("Product not found");

                    var newItem = new CartItem
                    {
                        CartId = cart.CartId,
                        ProductId = productId,
                        Quantity = quantity,
                        Price = product.Price * quantity
                    };
                    await _context.CartItems.AddAsync(newItem);
                }
            }

            // Cập nhật lại tổng giá trị giỏ hàng
            cart.TotalPrice = cart.CartItems.Sum(x => x.Product.Price * x.Quantity);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartItemAsync(int userId,int productId)
        {
            var cart = await _context.Carts
          .Include(c => c.CartItems)
          .ThenInclude(ci => ci.Product)
          .FirstOrDefaultAsync(c => c.UserId == userId);
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cart != null)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                throw new Exception("Not found");
            }
           
           await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(int userId, int productId, int quantityChange, bool isAbsolute = false)
        {
            var cart = await _context.Carts
        .Include(c => c.CartItems)
        .ThenInclude(ci => ci.Product)
        .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                throw new Exception("Cart not found");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem == null && quantityChange > 0)
            {
                // Thêm mới sản phẩm vào giỏ
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                    throw new Exception("Product not found");

                cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = productId,
                    Quantity = isAbsolute ? quantityChange : 1,
                    Price = product.Price * (isAbsolute ? quantityChange : 1)
                };

                await _context.CartItems.AddAsync(cartItem);
            }
            else if (cartItem != null)
            {
                // Cập nhật số lượng
                if (isAbsolute)
                    cartItem.Quantity = quantityChange;
                else
                    cartItem.Quantity += quantityChange;

                if (cartItem.Quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }
                else
                {
                    cartItem.Price = cartItem.Product.Price * cartItem.Quantity;
                }
            }

            // Cập nhật lại tổng tiền giỏ hàng
            cart.TotalPrice = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity);

            await _context.SaveChangesAsync();
        }

       
    }
}

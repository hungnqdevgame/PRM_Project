using BLL.IService;
using DAL.IRepository;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public Task AddToCartAsync(int userId, int productId, int quantity)
        => _cartRepository.AddToCartAsync(userId, productId, quantity);

        public Task<Cart> GetCartAsync(int userId)
            => _cartRepository.GetCartAsync(userId);

        public Task<CartItem?> GetCartItemAsync(int cartId, int productId)
        =>_cartRepository.GetCartItemAsync(cartId, productId);

        public Task RemoveCartItemAsync(int userId, int productId)
        => _cartRepository.RemoveCartItemAsync(userId,productId);

        public Task SaveChangesAsync()
            => _cartRepository.SaveChangesAsync();

        public Task UpdateCartAsync(int userId, int productId, int quantity)
       => _cartRepository.UpdateCartAsync(userId, productId, quantity);

        public Task UpdateCartItemAsync(int userId, int productId, int quantityChange, bool isAbsolute = false)
        => _cartRepository.UpdateCartItemAsync(userId, productId, quantityChange, isAbsolute);
    }
}

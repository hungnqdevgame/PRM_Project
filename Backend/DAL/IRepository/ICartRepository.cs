using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface ICartRepository
    {
        Task<Cart> GetCartAsync(int userId);
        Task AddToCartAsync(int userId, int productId, int quantity);
        Task SaveChangesAsync();
        Task UpdateCartAsync(int userId, int productId, int quantity);

        Task RemoveCartItemAsync(int userId, int productId);
        Task<CartItem?> GetCartItemAsync(int cartId, int productId);
        Task UpdateCartItemAsync(int userId, int productId, int quantityChange, bool isAbsolute = false);
    }
}

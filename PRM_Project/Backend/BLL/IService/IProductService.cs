using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IService
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int productId);
        Task<Product> AddProductAsync(Product product);
        Task DeleteProductAsync(int productId);
        Task<Product> UpdateProductAsync(Product product);
    }
}

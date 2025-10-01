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
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<List<Product>> GetAllProductsAsync()
            => await _productRepository.GetAllProductsAsync();
        public async Task<Product> GetProductByIdAsync(int productId)
            => await _productRepository.GetProductByIdAsync(productId);
    }
}

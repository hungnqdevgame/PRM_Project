using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly SalesAppDBContext _context;
        public ProductRepository(SalesAppDBContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
            => await _context.Products.Include(p => p.Category).ToListAsync();


        public async Task<Product> GetProductByIdAsync(int productId)
      => await _context.Products.Include(p => p.Category)
        .FirstOrDefaultAsync(p => p.ProductId == productId);

        public async Task<Product> AddProductAsync(Product product)
        {
            var newProduct = new Product
            {
                ProductName = product.ProductName,
                BriefDescription = product.BriefDescription,
                FullDescription = product.FullDescription,
                TechnicalSpecifications = product.TechnicalSpecifications,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId
            };
            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            // Reload with Category included
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == newProduct.ProductId);
        }

        public async Task DeleteProductAsync(int productId)
        {
           var product = await _context.Products.FindAsync(productId);
            if(product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Not found");
            }
           
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.ProductId);
            if (existingProduct != null)
            {
                existingProduct.ProductName = product.ProductName;
                existingProduct.BriefDescription = product.BriefDescription;
                existingProduct.FullDescription = product.FullDescription;
                existingProduct.TechnicalSpecifications = product.TechnicalSpecifications;
                existingProduct.Price = product.Price;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.CategoryId = product.CategoryId;
                _context.Products.Update(existingProduct);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Not found");
            }
            return existingProduct;
        }
    }
}

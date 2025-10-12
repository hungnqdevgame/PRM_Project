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
            => await _context.Products.ToListAsync();
        

        public async Task<Product> GetProductByIdAsync(int productId)
     => await _context.Products.FindAsync(productId);
       
    }
}

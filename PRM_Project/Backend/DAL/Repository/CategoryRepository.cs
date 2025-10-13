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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SalesAppDBContext _context;
        public CategoryRepository(SalesAppDBContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
            => await _context.Categories.ToListAsync();

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
         => await _context.Categories.FindAsync(categoryId);

        public async Task<Category> AddCategoryAsync(Category category)
        {
            var newCategory = new Category
            {
                CategoryName = category.CategoryName,

            };
            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();
            return newCategory;
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Not found");
            }

        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(category.CategoryId);
            if (existingCategory == null)
            {
                throw new Exception("Category not found");
            }
            existingCategory.CategoryName = category.CategoryName;

            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();
            return existingCategory;
        }
    }
}

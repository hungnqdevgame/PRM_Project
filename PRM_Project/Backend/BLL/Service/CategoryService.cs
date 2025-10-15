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
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<List<Category>> GetAllCategoriesAsync()
            => await _categoryRepository.GetAllCategoriesAsync();
        public async Task<Category> GetCategoryByIdAsync(int categoryId)
         => await _categoryRepository.GetCategoryByIdAsync(categoryId);
        public async Task<Category> AddCategoryAsync(Category category)
            => await _categoryRepository.AddCategoryAsync(category);
        public async Task DeleteCategoryAsync(int categoryId)
            => await _categoryRepository.DeleteCategoryAsync(categoryId);
        public async Task<Category> UpdateCategoryAsync(Category category)
            => await _categoryRepository.UpdateCategoryAsync(category);
    }
}

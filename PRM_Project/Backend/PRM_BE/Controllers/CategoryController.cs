using BLL.IService;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace PRM_BE.Controllers
{
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return new OkObjectResult(categories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(category);
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return new BadRequestObjectResult("Category is null");
            }
            var newCategory = await _categoryService.AddCategoryAsync(category);
            return new OkObjectResult(newCategory);
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                return new OkObjectResult(new { message = "Category deleted successfully" });
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return new BadRequestObjectResult("Category is null");
            }
            try
            {
                var updatedCategory = await _categoryService.UpdateCategoryAsync(category);
                return new OkObjectResult(updatedCategory);
            }
            catch (Exception ex)
            {
                return new NotFoundObjectResult(ex.Message);
            }
        }
    }
}

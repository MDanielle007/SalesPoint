using SalesPoint.Models;

namespace SalesPoint.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> AddCategoryAsync (Category category);
        Task<Category> GetCategoryByIdAsync (int id);
        Task<IEnumerable<Category>> GetAllCategoriesAsync ();
        Task UpdateCategory (Category category);
        Task DeleteCategoryAsync (int id);
        Task<bool> CategoryExistsAsync(int id);
        Task<bool> CategoryNameExistsAsync(string name);
    }
}

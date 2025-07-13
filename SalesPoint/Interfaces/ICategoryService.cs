using SalesPoint.DTO;

namespace SalesPoint.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDTO> AddCategoryAsync(CategoryCreateDTO categoryDTO);
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
        Task<CategoryDTO> UpdateCategoryAsync(CategoryUpdateDTO categoryDTO);
        Task DeleteCategoryAsync(int id);
    }
}

using AutoMapper;
using SalesPoint.DTO;
using SalesPoint.Exceptions;
using SalesPoint.Interfaces;
using SalesPoint.Models;

namespace SalesPoint.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository categoryRepository, 
            IMapper mapper, 
            ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CategoryDTO> AddCategoryAsync(CategoryCreateDTO categoryDTO)
        {
            try
            {
                var categoryExist = await _categoryRepository.CategoryNameExistsAsync(categoryDTO.Name);
                if (categoryExist)
                {
                    throw new BadRequestException($"Category with name {categoryDTO.Name} already exists");
                }

                var category = _mapper.Map<Category>(categoryDTO);
                var addedCategory = await _categoryRepository.AddCategoryAsync(category);
                return _mapper.Map<CategoryDTO>(addedCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding category");
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync();
                return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                throw;
            }
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(id);

                if (category == null) return null;

                return _mapper.Map<CategoryDTO>(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting category with ID: {id}");
                throw;
            }
        }

        public async Task<CategoryDTO> UpdateCategoryAsync(CategoryUpdateDTO categoryDTO)
        {
            try
            {
                var existingCategory = await _categoryRepository.GetCategoryByIdAsync(categoryDTO.Id);

                if (existingCategory == null)
                {
                    throw new NotFoundException($"Category with ID {categoryDTO.Id} not found");
                }

                if (categoryDTO.Name != null) existingCategory.Name = categoryDTO.Name;

                await _categoryRepository.UpdateCategory(existingCategory);
                return _mapper.Map<CategoryDTO>(existingCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating category with ID {categoryDTO.Id}");
                throw;
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    throw new NotFoundException($"Category with ID {id} not found");
                }

                await _categoryRepository.DeleteCategoryAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product with ID {id}");
                throw;
            }
        }
    }
}

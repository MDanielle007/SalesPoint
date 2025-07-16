using Microsoft.EntityFrameworkCore;
using SalesPoint.Data;
using SalesPoint.Interfaces;
using SalesPoint.Models;

namespace SalesPoint.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }
        
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var query = _context.Categories.AsQueryable();
            return query.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var query = _context.Categories.AsQueryable();
            query = query.Where(p => !p.IsDeleted);
            return await query.ToListAsync();
        }

        public async Task UpdateCategory(Category category)
        {
            category.UpdatedAt = DateTime.Now;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await GetCategoryByIdAsync(id);
            if (category != null)
            {
                category.DeletedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            var category = await GetCategoryByIdAsync(id);

            return category != null;
        }

        public async Task<bool> CategoryNameExistsAsync(string name)
        {
            var category = _context.Categories.Where(c => c.Name == name).FirstOrDefault();

            return category != null;
        }


    }
}

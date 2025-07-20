using Microsoft.EntityFrameworkCore;
using SalesPoint.Data;
using SalesPoint.DTO;
using SalesPoint.Enums;
using SalesPoint.Interfaces;
using SalesPoint.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SalesPoint.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> GetProductByIdAsync(int id, bool includeCategory = false)
        {
            var query = _context.Products.AsQueryable();

            if (includeCategory)
            {
                query = query.Include(p => p.Category);
            }

            return query.FirstOrDefault(p => p.Id == id && !p.IsDeleted );
        }

        public async Task<Product?> GetProductByCodeAsync(string productCode, bool includeCategory = false)
        {
            var query = _context.Products.AsQueryable();

            if (includeCategory)
            {
                query = query.Include(p => p.Category);
            }

            return query.FirstOrDefault(p => p.ProductCode == productCode && !p.IsDeleted);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync (ProductFilterDto filter, bool includeCategory = false)
        {
            var query = _context.Products.AsQueryable();

            if (!includeCategory)
            {
                query = query.Include(p =>p.Category);
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(p => p.Name.Contains(filter.Name));
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.SellingPrice >= filter.MinPrice);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.SellingPrice <= filter.MaxPrice);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(p => p.Status == filter.Status);
            }
            else if (!filter.IncludeInactive)
            {
                query = query.Where(p => p.Status == ProductStatus.Active);
            }

            query = query.Where(p => !p.IsDeleted);

            return await query.ToListAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            product.UpdatedAt = DateTime.Now;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await GetProductByIdAsync(id);
            if (product != null)
            {
                product.DeletedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ProductExistsAsync(int productId)
        {
            var product = await GetProductByIdAsync(productId);
            return product != null;
        }

        public async Task<bool> ProductCodeExistsAsync(string productCode)
        {
            var product = await GetProductByCodeAsync(productCode);

            return product != null;
        }
    }
}

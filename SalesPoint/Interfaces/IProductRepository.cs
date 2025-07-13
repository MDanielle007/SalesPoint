using SalesPoint.DTO;
using SalesPoint.Models;

namespace SalesPoint.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> AddProductAsync(Product product);
        Task<Product?> GetProductByIdAsync(int id, bool includeCategory = false);
        Task<Product?> GetProductByCodeAsync(string productCode, bool includeCategory = false);
        Task<IEnumerable<Product>> GetProductsAsync(ProductFilterDto filter, bool includeCategory = false);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<bool> ProductExistsAsync(int productId);
        Task<bool> ProductCodeExistsAsync(string productCode);
    }
}

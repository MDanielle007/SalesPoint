using SalesPoint.DTO;
using SalesPoint.Models;

namespace SalesPoint.Interfaces
{
    public interface IProductService
    {
        Task<ProductDTO> AddProductAsync(ProductCreateDTO productDTO);
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<ProductDTO?> GetProductByCodeAsync(string code);
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync(ProductFilterDto filter);
        Task<ProductDTO> UpdateProductAsync (ProductUpdateDto productDTO);
        Task DeleteProductAsync (int id);
    }
}

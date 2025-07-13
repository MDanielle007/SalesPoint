using AutoMapper;
using SalesPoint.DTO;
using SalesPoint.Exceptions;
using SalesPoint.Interfaces;
using SalesPoint.Models;

namespace SalesPoint.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository, 
            ICategoryRepository categoryRepository, 
            IMapper mapper, 
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductDTO> AddProductAsync(ProductCreateDTO productDTO)
        {
            try
            {
                if (productDTO.CategoryId == null)
                {
                    throw new BadRequestException("Category ID cannot be null");
                }

                var categoryExist = await _categoryRepository.CategoryExistsAsync(productDTO.CategoryId);
                if (!categoryExist)
                {
                    throw new NotFoundException($"Category with ID {productDTO.CategoryId} not found");
                }

                var codeExist = await _productRepository.ProductCodeExistsAsync(productDTO.ProductCode);
                if (codeExist)
                {
                    throw new BadRequestException($"Product with code {productDTO.ProductCode} already exists");
                }

                if (productDTO.SellingPrice < productDTO.Cost)
                {
                    throw new BadRequestException("Selling price cannot be less than cost");
                }

                var product = _mapper.Map<Product>(productDTO);
                var addedProduct = await _productRepository.AddProductAsync(product);
                return _mapper.Map<ProductDTO>(addedProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product");
                throw;
            }
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id, includeCategory: true);

                if (product == null) return null;

                return _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product with ID: {id}");
                throw;
            }
        }

        public async Task<ProductDTO?> GetProductByCodeAsync(string code)
        {
            try
            {
                var product = await _productRepository.GetProductByCodeAsync(code, includeCategory: true);

                if (product == null) return null;

                return _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product with product code: {code}");
                throw;
            }
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync(ProductFilterDto filter)
        {
            try
            {
                var products = await _productRepository.GetProductsAsync(filter);
                return _mapper.Map<IEnumerable<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                throw;
            }
        }

        public async Task<ProductDTO> UpdateProductAsync(ProductUpdateDto productDTO)
        {
            try
            {
                var existingProduct = await _productRepository.GetProductByIdAsync(productDTO.Id);

                if (existingProduct == null)
                {
                    throw new NotFoundException($"Product with ID {productDTO.Id} not found");
                }

                if (productDTO.CategoryId.HasValue)
                {
                    var categoryExists = await _categoryRepository.CategoryExistsAsync(productDTO.CategoryId.Value);
                    if (!categoryExists)
                    {
                        throw new NotFoundException($"Category with ID {productDTO.CategoryId} not found");
                    }
                }

                if (!string.IsNullOrEmpty(productDTO.ProductCode) && productDTO.ProductCode != existingProduct.ProductCode)
                {
                    var codeExists = await _productRepository.ProductCodeExistsAsync(productDTO.ProductCode);
                    if (codeExists)
                    {
                        throw new BadRequestException($"Product with code {productDTO.ProductCode} already exists");
                    }
                }

                if (productDTO.Name != null) existingProduct.Name = productDTO.Name;
                if (productDTO.Description != null) existingProduct.Description = productDTO.Description;
                if (productDTO.CategoryId.HasValue) existingProduct.CategoryId = productDTO.CategoryId.Value;
                if (productDTO.Cost.HasValue) existingProduct.Cost = productDTO.Cost.Value;
                if (productDTO.SellingPrice.HasValue) existingProduct.SellingPrice = productDTO.SellingPrice.Value;
                if (productDTO.Quantity.HasValue) existingProduct.Quantity = productDTO.Quantity.Value;
                if (productDTO.Status.HasValue) existingProduct.Status = productDTO.Status.Value;

                if (existingProduct.SellingPrice < existingProduct.Cost)
                {
                    throw new BadRequestException("Selling price cannot be less than cost");
                }

                await _productRepository.UpdateProductAsync(existingProduct);
                return _mapper.Map<ProductDTO>(existingProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product with ID {productDTO.Id}");
                throw;
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                {
                    throw new NotFoundException($"Product with ID {id} not found");
                }

                await _productRepository.DeleteProductAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product with ID {id}");
                throw;
            }
        }
    }
}

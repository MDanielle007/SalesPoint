using SalesPoint.Enums;
using System.ComponentModel.DataAnnotations;

namespace SalesPoint.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public decimal Cost { get; set; }
        public decimal SellingPrice { get; set; }
        public int Quantity { get; set; }
        public ProductStatus Status { get; set; }
    }

    public class ProductCreateDTO
    {
        [Required, StringLength(255)]
        public string ProductCode { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Cost { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal SellingPrice { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.Active;
    }

    public class ProductUpdateDto
    {
        [Required]
        public int Id { get; set; }

        public string? ProductCode { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int? CategoryId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Cost { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SellingPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int? Quantity { get; set; }

        public ProductStatus? Status { get; set; }
    }

    public class ProductFilterDto
    {
        public string? Name { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public ProductStatus? Status { get; set; }
        public bool IncludeInactive { get; set; } = false;
    }
}

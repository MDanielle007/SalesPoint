using SalesPoint.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesPoint.Models
{
    public class Product : BaseEntity
    {
        public int Id { get; set; }

        public string ProductCode { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SellingPrice { get; set; }

        public int Quantity { get; set; }

        public ProductStatus Status { get; set; }
    }
}

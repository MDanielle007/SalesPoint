using SalesPoint.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesPoint.Models
{
    public class Transaction : BaseEntity
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ChangeAmount {  get; set; }
        
        public TransactionStatus Status { get; set; }

        public DateTime DateTime { get; set; }

        public ICollection<TransactionProduct> Products { get; set; }
    }
}

using SalesPoint.Enum;
using System.ComponentModel.DataAnnotations;

namespace SalesPoint.DTO
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TotalAmount {  get; set; }
        public decimal ChangeAmount { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime DateTime { get; set; }
        public List<TransactionProductDTO> Products { get; set; } = new List<TransactionProductDTO>();
    }

    public class TransactionCreateDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required, Range(0.01, double.MaxValue)]
        public decimal AmountPaid { get; set; }

        [Required]
        public List<TransactionProductCreateDTO> Products { get; set; } = new List<TransactionProductCreateDTO>();
    }

    public class TransactionFilterDTO
    {
        public int? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TransactionStatus? Status { get; set; }
        public decimal? MinimumAmount { get; set; }
        public decimal? MaximumAmount { get; set; }
    }
}

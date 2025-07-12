using System.ComponentModel.DataAnnotations.Schema;

namespace SalesPoint.Models
{
    public class AuditLog : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public string Action { get; set; }  // "Create", "Update", "Delete"
        public string Model { get; set; }   // e.g., "Product"
        public int RecordId { get; set; }
        public string Description { get; set; }
        public string Changes { get; set; } // JSON string
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
}

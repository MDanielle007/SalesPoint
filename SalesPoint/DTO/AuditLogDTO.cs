using SalesPoint.Models;

namespace SalesPoint.DTO
{
    public class AuditLogDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }  // "Create", "Update", "Delete"
        public string Model { get; set; }   // e.g., "Product"
        public int RecordId { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
    }
}

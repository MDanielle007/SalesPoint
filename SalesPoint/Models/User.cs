using SalesPoint.Enums;

namespace SalesPoint.Models
{
    public class User : IdentityBase
    {
        public UserRole Role { get; set; }
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}

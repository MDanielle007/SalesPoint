using SalesPoint.Enum;
using System.Runtime.CompilerServices;

namespace SalesPoint.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public UserRole Role { get; set; }

        public string EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; } = string.Empty;

        public string LastName { get; set; }

        public string Phone { get; set; }

    }
}

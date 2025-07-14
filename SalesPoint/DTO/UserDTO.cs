using SalesPoint.Enum;
using System.ComponentModel.DataAnnotations;

namespace SalesPoint.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; }
        public string Phone { get; set; }
    }

    public class UserCreateDTO
    {
        [Required, StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Required, StringLength(50, MinimumLength = 10)]
        public string EmployeeId { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [Phone, StringLength(20)]
        public string Phone { get; set; }
    }

    public class UserUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? Password { get; set; }

        public UserRole? Role { get; set; }

        [StringLength(50)]
        public string? EmployeeId { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [Phone, StringLength(20)]
        public string? Phone { get; set; }
    }

    public class LoginDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public UserDTO User { get; set; }
    }

    public class ChangePasswordDTO
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required, StringLength(100, MinimumLength = 8)]
        public string NewPassword { get; set; }
    }

}

using SalesPoint.Models;

namespace SalesPoint.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByEmployeeIdAsync(string employeeId);
        Task<IEnumerable<User>> GetUsersAsync();
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(int id);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> EmployeeIdExistsAsync(string employeeId);
    }
}

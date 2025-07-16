using SalesPoint.Models;

namespace SalesPoint.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmployeeIdAsync(string employeeId);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<bool> EmployeeIdExistsAsync(string employeeId);
    }
}

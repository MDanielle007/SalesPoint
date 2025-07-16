using Microsoft.EntityFrameworkCore;
using SalesPoint.Data;
using SalesPoint.Interfaces;
using SalesPoint.Models;

namespace SalesPoint.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmployeeIdAsync(string employeeId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.EmployeeId == employeeId && !u.IsDeleted);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.Where( u => !u.IsDeleted).ToListAsync();
        }

        public async Task<bool> EmployeeIdExistsAsync(string employeeId)
        {
            return await _context.Users.AnyAsync(u => u.EmployeeId == employeeId);
        }
    }
}

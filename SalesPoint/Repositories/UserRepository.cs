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

        public async Task<User> AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User?> GetUserByEmployeeIdAsync(string employeeId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.EmployeeId == employeeId && !u.IsDeleted);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.Where( u => !u.IsDeleted).ToListAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.Now;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user != null)
            {
                user.DeletedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> EmployeeIdExistsAsync(string employeeId)
        {
            return await _context.Users.AnyAsync(u => u.EmployeeId == employeeId);
        }
    }
}

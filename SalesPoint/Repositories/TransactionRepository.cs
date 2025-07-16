using Microsoft.EntityFrameworkCore;
using SalesPoint.Data;
using SalesPoint.DTO;
using SalesPoint.Interfaces;
using SalesPoint.Models;

namespace SalesPoint.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction> AddTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int transactionId, bool includeDetails = false)
        {
            var query = _context.Transactions.AsQueryable();

            if (includeDetails)
            {
                query = query.Include(u => u.User).Include(p => p.Products).ThenInclude(tp => tp.Product);
            }

            return await query.FirstOrDefaultAsync(t => t.Id == transactionId && !t.IsDeleted);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync(TransactionFilterDTO transactionFilterDTO, bool includeDetails = false)
        {
            var query = _context.Transactions.AsQueryable();

            if (includeDetails)
            {
                query = query.Include(u => u.User).Include(p => p.Products).ThenInclude(tp => tp.Product);
            }

            if (!string.IsNullOrEmpty(transactionFilterDTO.UserId))
            {
                query = query.Where(t => t.UserId == transactionFilterDTO.UserId);
            }

            if (transactionFilterDTO.StartDate.HasValue)
            {
                query = query.Where(t => t.DateTime >= transactionFilterDTO.StartDate.Value);
            }

            if (transactionFilterDTO.EndDate.HasValue)
            {
                query = query.Where(t => t.DateTime <= transactionFilterDTO.EndDate.Value);
            }

            if (transactionFilterDTO.Status.HasValue)
            {
                query = query.Where(t => t.Status == transactionFilterDTO.Status.Value);
            }

            if (transactionFilterDTO.MinimumAmount.HasValue)
            {
                query = query.Where(t => t.TotalAmount >= transactionFilterDTO.MinimumAmount.Value);
            }

            if (transactionFilterDTO.MaximumAmount.HasValue)
            {
                query = query.Where(t => t.TotalAmount <= transactionFilterDTO.MaximumAmount.Value);
            }

            query = query.Where(t => !t.IsDeleted);

            return await query.ToListAsync();
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            transaction.UpdatedAt = DateTime.Now;
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TransactionExistAsync(int transactionId)
        {
            return await _context.Transactions.AnyAsync(t => t.Id == transactionId && !t.IsDeleted);
        }
    }
}

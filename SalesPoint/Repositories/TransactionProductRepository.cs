using Microsoft.EntityFrameworkCore;
using SalesPoint.Data;
using SalesPoint.Interfaces;
using SalesPoint.Models;

namespace SalesPoint.Repositories
{
    public class TransactionProductRepository : ITransactionProductRepository
    {
        private readonly AppDbContext _context;

        public TransactionProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddTransactionProductAsync(IEnumerable<TransactionProduct> transactionProducts)
        {
            await _context.TransactionProducts.AddRangeAsync(transactionProducts);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TransactionProduct>> GetTransactionProductsAsync(int transactionId)
        {
            return await _context.TransactionProducts
                .Include(tp => tp.Product)
                .Include(tp => tp.TransactionId == transactionId)
                .ToListAsync();
        }
    }
}

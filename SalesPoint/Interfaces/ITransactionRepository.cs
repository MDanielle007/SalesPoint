using SalesPoint.DTO;
using SalesPoint.Models;

namespace SalesPoint.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> AddTransactionAsync(Transaction transaction);
        Task<Transaction?> GetTransactionByIdAsync(int transactionId, bool includeDetails = false);
        Task<IEnumerable<Transaction>> GetTransactionsAsync(TransactionFilterDTO transactionFilterDTO, bool includeDetails = false);
        Task UpdateTransactionAsync(Transaction transaction);
        Task<bool> TransactionExistAsync(int transactionId);
    }
}

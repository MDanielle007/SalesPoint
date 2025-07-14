using SalesPoint.Models;

namespace SalesPoint.Interfaces
{
    public interface ITransactionProductRepository
    {
        Task AddTransactionProductAsync(IEnumerable<TransactionProduct> transactionProducts);
        Task<IEnumerable<TransactionProduct>> GetTransactionProductsAsync(int transactionId);
    }
}

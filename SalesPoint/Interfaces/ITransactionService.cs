using SalesPoint.DTO;

namespace SalesPoint.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionDTO> CreateTransactionAsync(TransactionCreateDTO transactionDTO);
        Task<TransactionDTO> GetTransactionByIdAsync(int  transactionId);
        Task<IEnumerable<TransactionDTO>> GetTransactionsAsync(TransactionFilterDTO filter);
        Task CancelTransactionAsync(int transactionId);
    }
}

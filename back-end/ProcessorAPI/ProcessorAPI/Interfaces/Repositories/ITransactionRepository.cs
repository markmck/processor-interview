using ProcessorAPI.Models;
using ProcessorAPI.Models.Enums;

namespace ProcessorAPI.Interfaces.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllAsync(CardType? cardType = null, TransactionStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<Transaction?> GetByIdAsync(int id);
        Task<int> AddRangeAsync(IEnumerable<Transaction> transactions);
    }
}
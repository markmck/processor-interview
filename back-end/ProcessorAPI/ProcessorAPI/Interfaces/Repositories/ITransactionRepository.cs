using ProcessorAPI.Models;
using ProcessorAPI.Models.Enums;

namespace ProcessorAPI.Interfaces.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllAsync(string? cardNumber = null, CardType? cardType = null, TransactionStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<int> AddRangeAsync(IEnumerable<Transaction> transactions);
    }
}
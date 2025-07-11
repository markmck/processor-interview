using ProcessorAPI.Models;

namespace ProcessorAPI.Interfaces.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<Transaction?> GetByIdAsync(int id);
        Task<bool> AddRangeAsync(IEnumerable<Transaction> transactions);
    }
}
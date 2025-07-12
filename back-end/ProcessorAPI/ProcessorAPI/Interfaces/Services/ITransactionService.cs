using ProcessorAPI.Models;
using ProcessorAPI.Models.Results;

namespace ProcessorAPI.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<Transaction?> GetByIdAsync(int id);
        Task<ProcessingResult> ProcessTransactionsAsync(string data, string contentType);
    }
}
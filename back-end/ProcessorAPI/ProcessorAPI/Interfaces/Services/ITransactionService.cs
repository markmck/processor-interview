using ProcessorAPI.Models;

namespace ProcessorAPI.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<Transaction?> GetByIdAsync(int id);
        Task<bool> HandleXmlUpload(IEnumerable<Transaction> transactions);
        Task<bool> HandleCsvUpload(IEnumerable<Transaction> transactions);
        Task<bool> HandleJsonUpload(IEnumerable<Transaction> transactions);
    }
}
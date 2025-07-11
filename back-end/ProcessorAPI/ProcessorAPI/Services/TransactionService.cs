using ProcessorAPI.Interfaces.Repositories;
using ProcessorAPI.Interfaces.Services;
using ProcessorAPI.Models;

namespace ProcessorAPI.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository transactionRepository;

        public TransactionService(ITransactionRepository _transactionRepository)
        {
            this.transactionRepository = _transactionRepository;
        }

        public Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return transactionRepository.GetAllAsync();
        }

        public Task<Transaction?> GetByIdAsync(int id)
        {
            return transactionRepository.GetByIdAsync(id);
        }

        public Task<bool> HandleCsvUpload(IEnumerable<Transaction> transactions)
        {
            
            
            throw new NotImplementedException();
        }

        public Task<bool> HandleJsonUpload(IEnumerable<Transaction> transactions)
        {
            return transactionRepository.AddRangeAsync(transactions);
        }

        public Task<bool> HandleXmlUpload(IEnumerable<Transaction> transactions)
        {
            throw new NotImplementedException();
        }

    }
}

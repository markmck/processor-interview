using Microsoft.EntityFrameworkCore;
using ProcessorAPI.Interfaces.Repositories;
using ProcessorAPI.Models;

namespace ProcessorAPI.Data.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ProcessorDbContext _context;

        public TransactionRepository(ProcessorDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task<int> AddRangeAsync(IEnumerable<Transaction> transactions)
        {
            _context.Transactions.AddRange(transactions);
            var result = await _context.SaveChangesAsync();

            return result;
        }
    }
}

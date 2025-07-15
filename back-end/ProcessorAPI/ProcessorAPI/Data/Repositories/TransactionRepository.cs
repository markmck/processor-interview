using Microsoft.EntityFrameworkCore;
using ProcessorAPI.Interfaces.Repositories;
using ProcessorAPI.Models;
using ProcessorAPI.Models.Enums;

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

        public async Task<IEnumerable<Transaction>> GetAllAsync(string? cardNumber = null, CardType? cardType = null, TransactionStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Transactions.AsNoTracking().AsQueryable();

            if(!string.IsNullOrEmpty(cardNumber))
                query = query.Where(t => t.CardNumber == cardNumber);

            if (cardType.HasValue)
                query = query.Where(t => t.CardType == cardType.Value);

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (fromDate.HasValue)
                query = query.Where(t => t.TimeStamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(t => t.TimeStamp <= toDate.Value);

            return await query.ToListAsync();
        }

        public async Task<int> AddRangeAsync(IEnumerable<Transaction> transactions)
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Transactions.AddRange(transactions);
                var result = await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                return result;
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }
    }
}

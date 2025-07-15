using Microsoft.AspNetCore.Mvc;
using ProcessorAPI.Models;
using ProcessorAPI.Models.Enums;
using ProcessorAPI.Models.Results;

namespace ProcessorAPI.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetAllAsync(CardType? cardType = null,
            TransactionStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);
        Task<ProcessingResult> ProcessTransactionsAsync(string data, string contentType);
    }
}
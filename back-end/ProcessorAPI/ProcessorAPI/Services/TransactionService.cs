using ProcessorAPI.Helpers;
using ProcessorAPI.Interfaces.Helpers;
using ProcessorAPI.Interfaces.Repositories;
using ProcessorAPI.Interfaces.Services;
using ProcessorAPI.Models;
using ProcessorAPI.Models.Enums;
using ProcessorAPI.Models.Results;
using System.Text.Json;

namespace ProcessorAPI.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IJSONParser jsonParser;
        private readonly IXMLParser xmlParser;
        private readonly ICSVParser csvParser;
        private readonly ITransactionValidator transactionValidator;
        private readonly ILogger<TransactionService> logger;

        public TransactionService(
            ITransactionRepository _transactionRepository,
            IJSONParser _jsonParser,
            IXMLParser _xmlParser,
            ICSVParser _csvParser,
            ITransactionValidator _transactionValidator,
            ILogger<TransactionService> _logger)
        {
            this.transactionRepository = _transactionRepository;
            this.jsonParser = _jsonParser;
            this.xmlParser = _xmlParser;
            this.csvParser = _csvParser;
            this.transactionValidator = _transactionValidator;
            this.logger = _logger;
        }

        public Task<IEnumerable<Transaction>> GetAllAsync(CardType? cardType = null, TransactionStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            return transactionRepository.GetAllAsync(cardType, status, fromDate, toDate);
        }

        public async Task<ProcessingResult> ProcessTransactionsAsync(string data, string contentType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data))
                {
                    return new ProcessingResult { Success = false, ErrorMessage = "Input data cannot be empty" };
                }

                List<Transaction> transactions = contentType.ToLower() switch
                {
                    var ct when ct.Contains("json") => jsonParser.ParseJSONTransactions(data),
                    var ct when ct.Contains("xml") => xmlParser.ParseXMLTransactions(data),
                    var ct when ct.Contains("csv") => csvParser.ParseCSVTransactions(data),
                    _ => throw new NotSupportedException($"Content type '{contentType}' is not supported")
                };

                foreach (var transaction in transactions)
                {
                    var validationResult = transactionValidator.Validate(transaction);

                    if (validationResult.IsValid)
                    {
                        transaction.Status = TransactionStatus.Accepted;
                        transaction.CardType = validationResult.CardType;
                    }
                    else
                    {
                        transaction.Status = TransactionStatus.Rejected;
                        transaction.CardType = CardType.Invalid;
                    }
                }

                var results = await transactionRepository.AddRangeAsync(transactions);

                return new ProcessingResult
                {
                    Success = true,
                    ProcessedCount = results
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing transactions");
                return new ProcessingResult { Success = false, ErrorMessage = $"Processing failed: {ex.Message}" };
            }
        }
    }
}
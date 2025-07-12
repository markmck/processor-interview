using ProcessorAPI.Helpers;
using ProcessorAPI.Interfaces.Helpers;
using ProcessorAPI.Interfaces.Repositories;
using ProcessorAPI.Interfaces.Services;
using ProcessorAPI.Models;
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
        private readonly ILogger<TransactionService> logger;

        public TransactionService(
            ITransactionRepository _transactionRepository,
            IJSONParser _jsonParser,
            IXMLParser _xmlParser,
            ICSVParser _csvParser,
            ILogger<TransactionService> _logger)
        {
            this.transactionRepository = _transactionRepository;
            this.jsonParser = _jsonParser;
            this.xmlParser = _xmlParser;
            this.csvParser = _csvParser;
            this.logger = _logger;
        }

        public Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return transactionRepository.GetAllAsync();
        }

        public Task<Transaction?> GetByIdAsync(int id)
        {
            return transactionRepository.GetByIdAsync(id);
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
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
        private readonly ILogger<TransactionService> logger;

        public TransactionService(
            ITransactionRepository transactionRepository,
            ILogger<TransactionService> logger)
        {
            this.transactionRepository = transactionRepository;
            this.logger = logger;
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

                var transactions = contentType.ToLower() switch
                {
                    var ct when ct.Contains("json") => ParseJsonTransactions(data),
                    var ct when ct.Contains("xml") => ParseXmlTransactions(data),
                    var ct when ct.Contains("csv") => ParseCsvTransactions(data),
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

        //TODO: Move this to parser class
        private List<Transaction> ParseJsonTransactions(string jsonData)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            try
            {
                return JsonSerializer.Deserialize<List<Transaction>>(jsonData, options)
                    ?? new List<Transaction>();
            }
            catch (JsonException)
            {
                var single = JsonSerializer.Deserialize<Transaction>(jsonData, options);
                return single != null ? new List<Transaction> { single } : new List<Transaction>();
            }
        }

        private List<Transaction> ParseXmlTransactions(string xmlData)
        {
            throw new NotImplementedException("XML parsing not yet implemented");
        }

        private List<Transaction> ParseCsvTransactions(string csvData)
        {
            throw new NotImplementedException("XML parsing not yet implemented");
        }
    }
}
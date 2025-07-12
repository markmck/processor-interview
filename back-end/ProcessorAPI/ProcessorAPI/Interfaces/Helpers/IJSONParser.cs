using ProcessorAPI.Models;

namespace ProcessorAPI.Helpers
{
    public interface IJSONParser
    {
        public List<Transaction> ParseJSONTransactions(string jsonData);
    }
}

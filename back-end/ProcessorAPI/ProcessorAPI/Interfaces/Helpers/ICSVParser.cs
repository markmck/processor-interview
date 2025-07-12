using ProcessorAPI.Models;

namespace ProcessorAPI.Interfaces.Helpers
{
    public interface ICSVParser
    {
        public List<Transaction> ParseCSVTransactions(string jsonData);
    }
}

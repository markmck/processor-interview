using ProcessorAPI.Models;

namespace ProcessorAPI.Interfaces.Helpers
{
    public interface IXMLParser
    {
        public List<Transaction> ParseXMLTransactions(string jsonData);
    }
}

using ProcessorAPI.Models;
using System.Text.Json;

namespace ProcessorAPI.Helpers
{
    public class JSONParser : IJSONParser
    {
        public List<Transaction> ParseJSONTransactions(string jsonData)
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
    }
}

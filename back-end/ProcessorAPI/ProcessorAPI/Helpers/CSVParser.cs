using CsvHelper;
using ProcessorAPI.Interfaces.Helpers;
using ProcessorAPI.Models;
using System.Formats.Asn1;
using System.Globalization;

namespace ProcessorAPI.Helpers
{
    public class CSVParser : ICSVParser
    {
        public List<Transaction> ParseCSVTransactions(string csvData)
        {
            using var stringReader = new StringReader(csvData);
            using var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<TransactionMap>();
            csv.Context.Configuration.HasHeaderRecord = true;
            csv.Context.Configuration.IgnoreBlankLines = true;
            csv.Context.Configuration.TrimOptions = CsvHelper.Configuration.TrimOptions.Trim;
            csv.Context.Configuration.MissingFieldFound = null; // Ignore missing fields

            return csv.GetRecords<Transaction>().ToList();
        }
    }
}

// CSV mapping configuration, This could go in a separate file but it's fine here for our use. 
public sealed class TransactionMap : CsvHelper.Configuration.ClassMap<Transaction>
{
    public TransactionMap()
    {
        // Map CSV columns to Transaction properties
        Map(m => m.Id).Name("Id", "TransactionId", "ID").Optional();
        Map(m => m.CardNumber).Name("cardNumber");
        Map(m => m.Amount).Name("amount");
        Map(m => m.TimeStamp).Name("timestamp");
    }
}
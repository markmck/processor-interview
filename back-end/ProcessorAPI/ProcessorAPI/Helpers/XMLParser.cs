using ProcessorAPI.Interfaces.Helpers;
using ProcessorAPI.Models;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace ProcessorAPI.Helpers
{
    public class XMLParser : IXMLParser
    {
        public List<Transaction> ParseXMLTransactions(string xmlData)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(TransactionList));
                using var stringReader = new StringReader(xmlData);
                var transactionList = serializer.Deserialize(stringReader) as TransactionList;
                return transactionList?.Transactions ?? new List<Transaction>();
            }
            catch (InvalidOperationException)
            {
                // Try single transaction
                var serializer = new XmlSerializer(typeof(Transaction));
                using var stringReader = new StringReader(xmlData);
                var transaction = serializer.Deserialize(stringReader) as Transaction;
                return transaction != null ? new List<Transaction> { transaction } : new List<Transaction>();
            }
        }
    }
}

[XmlRoot("transactions")]
public class TransactionList
{
    [XmlElement("transaction")]
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
}
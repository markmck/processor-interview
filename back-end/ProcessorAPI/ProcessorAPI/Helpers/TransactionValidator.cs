using ProcessorAPI.Interfaces.Helpers;
using ProcessorAPI.Models;
using ProcessorAPI.Models.Enums;
using ProcessorAPI.Models.Results;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace ProcessorAPI.Helpers
{
    public class TransactionValidator : ITransactionValidator
    {
        public CardType GetCardType(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 16)
                return CardType.Invalid;

            switch(cardNumber[0])
            {
                case '3':
                    return CardType.Amex;
                case '4':
                    return CardType.Visa;
                case '5':
                    return CardType.MasterCard;
                case '6':
                    return CardType.Discover;
                default:
                    return CardType.Invalid;
            }
        }

        public ValidationResult Validate(Transaction transaction)
        {
            if (!transaction.CardNumber.All(char.IsDigit))
            {
                return new ValidationResult { IsValid = false, CardType = CardType.Invalid };
            }

            if (transaction.CardNumber.Length != 16)
            {
                return new ValidationResult { IsValid = false, CardType = CardType.Invalid };
            }

            var cardType = GetCardType(transaction.CardNumber);
            if (cardType == CardType.Invalid)
            {
                return new ValidationResult { IsValid = false, CardType = CardType.Invalid };
            }

            if (transaction.Amount == 0)
            {
                return new ValidationResult { IsValid = false, CardType = cardType };
            }

            if (transaction.TimeStamp == default(DateTime))
            {
                return new ValidationResult { IsValid = false, CardType = cardType };
            }

            return new ValidationResult { IsValid = true, CardType = cardType };
        }
    }
}

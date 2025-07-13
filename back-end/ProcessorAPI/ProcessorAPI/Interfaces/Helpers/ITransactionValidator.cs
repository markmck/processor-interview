using ProcessorAPI.Models;
using ProcessorAPI.Models.Results;

namespace ProcessorAPI.Interfaces.Helpers
{
    public interface ITransactionValidator
    {
        public ValidationResult Validate(Transaction transaction);

        public Models.Enums.CardType GetCardType(string cardNumber);

    }
}

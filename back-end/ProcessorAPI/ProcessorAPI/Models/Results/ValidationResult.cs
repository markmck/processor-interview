using ProcessorAPI.Models.Enums;

namespace ProcessorAPI.Models.Results
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public CardType CardType { get; set; }
    }
}
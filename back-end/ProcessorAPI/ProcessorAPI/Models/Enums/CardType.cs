using System.ComponentModel;

namespace ProcessorAPI.Models.Enums
{
    /// <summary>
    /// Represents the type of credit card based on the card number prefix
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// Invalid or unrecognized card type (0)
        /// </summary>
        [Description("Invalid or unrecognized card type")]
        Invalid = 0,

        /// <summary>
        /// American Express card (3)
        /// </summary>
        [Description("American Express - Cards starting with 3")]
        Amex = 3,

        /// <summary>
        /// Visa card (4)
        /// </summary>
        [Description("Visa - Cards starting with 4")]
        Visa = 4,

        /// <summary>
        /// MasterCard (5)
        /// </summary>
        [Description("MasterCard - Cards starting with 5")]
        MasterCard = 5,

        /// <summary>
        /// Discover card (6)
        /// </summary>
        [Description("Discover - Cards starting with 6")]
        Discover = 6
    }
}

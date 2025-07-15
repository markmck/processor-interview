using System.ComponentModel;

namespace ProcessorAPI.Models.Enums
{
    /// <summary>
    /// Represents the processing status of a credit card transaction
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// Transaction was accepted and processed successfully (1)
        /// </summary>
        [Description("Transaction accepted and processed successfully")]
        Accepted = 1,

        /// <summary>
        /// Transaction was rejected due to validation or processing errors (2)
        /// </summary>
        [Description("Transaction rejected due to validation or processing errors")]
        Rejected = 2
    }
}

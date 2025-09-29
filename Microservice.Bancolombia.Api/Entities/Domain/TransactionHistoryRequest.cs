using System.ComponentModel.DataAnnotations;

namespace Microservice.Bancolombia.Api.Entities.Domain
{
    /// <summary>
    /// Represents a request model for transaction operations within the Bancolombia system.
    /// This class encapsulates all necessary data required to create a new transaction history record.
    /// </summary>
    public class TransactionHistoryRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier for the Transaction History.
        /// This identifier is used to reference the Transaction History throughout the system.
        /// </summary>
        /// <value>A long integer representing the Transaction History ID.</value>
        public long TransactionHistoryId { get; set; }

        /// <summary>
        /// The identifier of the account from which the transaction originates.
        /// </summary>
        [Required]
        public int FromAccountId { get; set; }

        /// <summary>
        /// The identifier of the destination account for the transaction.
        /// </summary>
        [Required]
        public int ToAccountId { get; set; }

        /// <summary>
        /// The type of transaction (e.g., TRF for transfer, DEP for deposit, WTH for withdrawal).
        /// </summary>
        [Required]
        [MaxLength(3)]
        public string TransactionType { get; set; } = string.Empty;

        /// <summary>
        /// The monetary amount of the transaction.
        /// </summary>
        [Required]
        public decimal Amount { get; set; }
    }
}
namespace Microservice.Bancolombia.Api.Entities.Domain
{
    /// <summary>
    /// Represents the response model containing transaction history information with customer details.
    /// </summary>
    public class TransactionHistoryResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the transaction.
        /// </summary>
        public long TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the account from which the transaction originated.
        /// </summary>
        public int FromAccountId { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer who owns the origin account.
        /// </summary>
        public string FromAccountCustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the identifier of the destination account for the transaction.
        /// </summary>
        public int ToAccountId { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer who owns the destination account.
        /// </summary>
        public string ToAccountCustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of transaction (e.g., transfer, deposit, withdrawal).
        /// </summary>
        public string TransactionType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the amount of the transaction.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the transaction was executed.
        /// </summary>
        public DateTime TransactionDate { get; set; }
    }
}
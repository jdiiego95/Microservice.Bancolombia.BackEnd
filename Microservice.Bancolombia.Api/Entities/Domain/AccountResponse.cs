namespace Microservice.Bancolombia.Api.Entities.Domain
{
    /// <summary>
    /// Represents the response model containing account information.
    /// </summary>
    public class AccountResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the account.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer who owns the account.
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total balance of the account.
        /// </summary>
        public decimal TotalBalance { get; set; }
    }
}
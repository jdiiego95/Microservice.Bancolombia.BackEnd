using System.ComponentModel.DataAnnotations;

namespace Microservice.Bancolombia.Api.Entities.Domain
{
    /// <summary>
    /// Represents a request model for account operations within the Bancolombia system.
    /// This class encapsulates all necessary data required to create or update an account record.
    /// </summary>
    public class AccountRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier for the Account.
        /// This identifier is used to reference the Account throughout the system.
        /// </summary>
        /// <value>An integer representing the Account ID.</value>
        public int AccountId { get; set; }

        /// <summary>
        /// The name of the customer who owns the account.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// The total balance amount of the account.
        /// </summary>
        [Required]
        public decimal TotalBalance { get; set; }
    }
}
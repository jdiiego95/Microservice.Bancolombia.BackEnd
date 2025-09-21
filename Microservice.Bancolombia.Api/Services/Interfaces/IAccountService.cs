using Microservice.Bancolombia.Api.Entities.Domain;

namespace Microservice.Bancolombia.Api.Services.Interfaces
{
    /// <summary>
    /// Interface for services related to accounts.
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Retrieves accounts based on the specified account ID.
        /// </summary>
        /// <param name="accountId">Optional account ID to filter the accounts.</param>
        /// <returns>A list of account responses.</returns>
        IEnumerable<AccountResponse> GetAccounts(int? accountId);

        /// <summary>
        /// Creates a new account asynchronously.
        /// </summary>
        /// <param name="accountRequest">The request containing details of the account to create.</param>
        /// <returns>A task that represents the asynchronous operation, with a result indicating success.</returns>
        Task<string> CreateAccountAsync(AccountRequest accountRequest);

        /// <summary>
        /// Updates an existing account asynchronously.
        /// </summary>
        /// <param name="accountRequest">The request containing updated information.</param>
        /// <returns>A task that represents the asynchronous operation, with a result indicating success.</returns>
        Task<string> UpdateAccountAsync(AccountRequest accountRequest);

        /// <summary>
        /// Delete an existing account asynchronously.
        /// </summary>
        /// <param name="accountId">Account identifier.</param>
        /// <returns>A task representing the asynchronous operation, with a result indicating the success of the operation.</returns>
        Task<string> DeleteAccountAsync(int accountId);
    }
}
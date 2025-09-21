using System.Linq.Expressions;
using Microservice.Bancolombia.Api.Entities.Model;

namespace Microservice.Bancolombia.Api.Clients.Interfaces
{
    /// <summary>
    /// Interface for client operations related to accounts.
    /// </summary>
    public interface IAccountClient
    {
        /// <summary>
        /// Retrieves accounts based on a filter.
        /// </summary>
        /// <param name="filter">An expression to filter the accounts.</param>
        /// <returns>A collection of accounts that match the filter.</returns>
        IEnumerable<Account> GetAccounts(Expression<Func<Account, bool>>? filter);

        /// <summary>
        /// Retrieves a single account based on a filter.
        /// </summary>
        /// <param name="filter">An expression to filter the account.</param>
        /// <returns>The account that matches the filter, or null if no match is found.</returns>
        Account? GetAccount(Expression<Func<Account, bool>> filter);

        /// <summary>
        /// Creates a new account asynchronously.
        /// </summary>
        /// <param name="account">The account to create.</param>
        /// <returns>A task representing the asynchronous operation, with the created account.</returns>
        Task<Account> CreateAccountAsync(Account account);

        /// <summary>
        /// Updates an existing account asynchronously.
        /// </summary>
        /// <param name="account">The account to update.</param>
        /// <returns>A task representing the asynchronous operation, with the updated account.</returns>
        Task<Account> UpdateAccountAsync(Account account);

        /// <summary>
        /// Delete an existing account asynchronously.
        /// </summary>
        /// <param name="accountId">Account identifier.</param>
        Task DeleteAccountAsync(int accountId);
    }
}
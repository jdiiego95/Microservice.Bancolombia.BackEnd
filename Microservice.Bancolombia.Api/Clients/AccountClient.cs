using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microservice.Bancolombia.Api.Clients.Interfaces;
using Microservice.Bancolombia.Api.Entities;
using Microservice.Bancolombia.Api.Entities.Model;

namespace Microservice.Bancolombia.Api.Clients
{
    /// <summary>
    /// Client class for managing Account entity operations including retrieval, creation, update and deletion
    /// </summary>
    /// <remarks>
    /// This internal client provides Account entity operations using Entity Framework directly,
    /// utilizing execution strategies for reliable database operations
    /// </remarks>
    /// <param name="context">The Entity Framework context for database operations</param>
    /// <exception cref="ArgumentNullException">Thrown when context parameter is null</exception>
    internal class AccountClient : BaseClient, IAccountClient
    {
        /// <summary>
        /// Initializes a new instance of the AccountClient class
        /// </summary>
        /// <param name="context">The Entity Framework context for database operations</param>
        public AccountClient(MainContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a collection of accounts based on the specified filter criteria
        /// </summary>
        /// <param name="filter">Optional lambda expression to filter accounts. If null, returns all accounts</param>
        /// <returns>An enumerable collection of Account entities matching the filter criteria</returns>
        public IEnumerable<Account> GetAccounts(Expression<Func<Account, bool>>? filter)
        {
            var query = _context.Accounts.AsQueryable();
            return filter != null ? query.Where(filter) : query;
        }

        /// <summary>
        /// Retrieves a single account based on the specified filter criteria
        /// </summary>
        /// <param name="filter">Lambda expression to identify the specific account to retrieve</param>
        /// <returns>The Account entity matching the filter criteria, or null if no match is found</returns>
        public Account? GetAccount(Expression<Func<Account, bool>> filter)
        {
            return _context.Accounts.FirstOrDefault(filter);
        }

        /// <summary>
        /// Creates a new account entity asynchronously with resilient execution strategy
        /// </summary>
        /// <param name="account">The Account entity to be created in the database</param>
        /// <returns>A task containing the created Account entity</returns>
        /// <remarks>
        /// Uses Entity Framework's execution strategy to handle transient failures during database operations
        /// </remarks>
        public async Task<Account> CreateAccountAsync(Account account)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return account;
            });
        }

        /// <summary>
        /// Updates an existing account entity asynchronously
        /// </summary>
        /// <param name="account">The Account entity to be updated in the database</param>
        /// <returns>A task containing the updated Account entity</returns>
        public async Task<Account> UpdateAccountAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return account;
        }

        /// <summary>
        /// Deletes an existing account entity asynchronously
        /// </summary>
        /// <param name="accountId">The identifier of the account to be deleted</param>
        /// <returns>A task representing the asynchronous delete operation</returns>
        public async Task DeleteAccountAsync(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId).ConfigureAwait(false);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
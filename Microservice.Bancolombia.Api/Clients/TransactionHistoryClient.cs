using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microservice.Bancolombia.Api.Clients.Interfaces;
using Microservice.Bancolombia.Api.Entities;
using Microservice.Bancolombia.Api.Entities.Model;

namespace Microservice.Bancolombia.Api.Clients
{
    /// <summary>
    /// Client class for managing TransactionHistory entity operations including retrieval and creation
    /// </summary>
    /// <remarks>
    /// This internal client provides TransactionHistory entity operations using Entity Framework directly,
    /// utilizing execution strategies for reliable database operations
    /// </remarks>
    /// <param name="context">The Entity Framework context for database operations</param>
    /// <exception cref="ArgumentNullException">Thrown when context parameter is null</exception>
    internal class TransactionHistoryClient : BaseClient, ITransactionHistoryClient
    {
        /// <summary>
        /// Initializes a new instance of the TransactionHistoryClient class
        /// </summary>
        /// <param name="context">The Entity Framework context for database operations</param>
        public TransactionHistoryClient(MainContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a collection of transaction histories based on the specified filter criteria
        /// </summary>
        /// <param name="filter">Optional lambda expression to filter transaction histories. If null, returns all transaction histories</param>
        /// <returns>An enumerable collection of TransactionHistory entities matching the filter criteria</returns>
        public IEnumerable<TransactionHistory> GetTransactionHistories(Expression<Func<TransactionHistory, bool>>? filter)
        {
            var query = _context.TransactionHistories.AsQueryable();
            return filter != null ? query.Where(filter) : query;
        }

        /// <summary>
        /// Retrieves a single transaction history based on the specified filter criteria
        /// </summary>
        /// <param name="filter">Lambda expression to identify the specific transaction history to retrieve</param>
        /// <returns>The TransactionHistory entity matching the filter criteria, or null if no match is found</returns>
        public TransactionHistory? GetTransactionHistory(Expression<Func<TransactionHistory, bool>> filter)
        {
            return _context.TransactionHistories.FirstOrDefault(filter);
        }

        /// <summary>
        /// Retrieves transaction histories by destination account identifier with related account information
        /// </summary>
        /// <param name="toAccountId">The identifier of the destination account</param>
        /// <returns>An enumerable collection of TransactionHistory entities for the specified destination account with account details</returns>
        public IEnumerable<TransactionHistory> GetTransactionHistoriesByToAccountId(int toAccountId)
        {
            return _context.TransactionHistories
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .Where(t => t.ToAccountId == toAccountId)
                .OrderByDescending(t => t.TransactionDate);
        }

        /// <summary>
        /// Retrieves transaction histories with related account information based on the specified filter criteria
        /// </summary>
        /// <param name="filter">Optional lambda expression to filter transaction histories. If null, returns all transaction histories with account details</param>
        /// <returns>An enumerable collection of TransactionHistory entities with account details matching the filter criteria</returns>
        public IEnumerable<TransactionHistory> GetTransactionHistoriesWithAccount(Expression<Func<TransactionHistory, bool>>? filter)
        {
            var query = _context.TransactionHistories
                .Include(t => t.FromAccount)
                .Include(t => t.ToAccount)
                .AsQueryable();
            return filter != null ? query.Where(filter) : query;
        }

        /// <summary>
        /// Creates a new transaction history entity asynchronously with resilient execution strategy
        /// </summary>
        /// <param name="transactionHistory">The TransactionHistory entity to be created in the database</param>
        /// <returns>A task containing the created TransactionHistory entity</returns>
        /// <remarks>
        /// Uses Entity Framework's execution strategy to handle transient failures during database operations
        /// </remarks>
        public async Task<TransactionHistory> CreateTransactionHistoryAsync(TransactionHistory transactionHistory)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                _context.TransactionHistories.Add(transactionHistory);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return transactionHistory;
            });
        }
    }
}
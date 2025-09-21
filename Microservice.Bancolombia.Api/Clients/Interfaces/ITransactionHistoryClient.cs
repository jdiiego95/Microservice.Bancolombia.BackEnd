using System.Linq.Expressions;
using Microservice.Bancolombia.Api.Entities.Model;

namespace Microservice.Bancolombia.Api.Clients.Interfaces
{
    /// <summary>
    /// Interface for client operations related to transaction history.
    /// </summary>
    public interface ITransactionHistoryClient
    {
        /// <summary>
        /// Retrieves transaction histories based on a filter.
        /// </summary>
        /// <param name="filter">An expression to filter the transaction histories.</param>
        /// <returns>A collection of transaction histories that match the filter.</returns>
        IEnumerable<TransactionHistory> GetTransactionHistories(Expression<Func<TransactionHistory, bool>>? filter);

        /// <summary>
        /// Retrieves a single transaction history based on a filter.
        /// </summary>
        /// <param name="filter">An expression to filter the transaction history.</param>
        /// <returns>The transaction history that matches the filter, or null if no match is found.</returns>
        TransactionHistory? GetTransactionHistory(Expression<Func<TransactionHistory, bool>> filter);

        /// <summary>
        /// Retrieves transaction histories by destination account identifier with related account information.
        /// </summary>
        /// <param name="toAccountId">The identifier of the destination account.</param>
        /// <returns>A collection of transaction histories for the specified destination account with account details.</returns>
        IEnumerable<TransactionHistory> GetTransactionHistoriesByToAccountId(int toAccountId);

        /// <summary>
        /// Retrieves transaction histories with related account information based on a filter.
        /// </summary>
        /// <param name="filter">An expression to filter the transaction histories.</param>
        /// <returns>A collection of transaction histories with account details that match the filter.</returns>
        IEnumerable<TransactionHistory> GetTransactionHistoriesWithAccount(Expression<Func<TransactionHistory, bool>>? filter);

        /// <summary>
        /// Creates a new transaction history asynchronously.
        /// </summary>
        /// <param name="transactionHistory">The transaction history to create.</param>
        /// <returns>A task representing the asynchronous operation, with the created transaction history.</returns>
        Task<TransactionHistory> CreateTransactionHistoryAsync(TransactionHistory transactionHistory);
    }
}
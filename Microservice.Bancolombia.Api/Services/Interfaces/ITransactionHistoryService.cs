using Microservice.Bancolombia.Api.Entities.Domain;

namespace Microservice.Bancolombia.Api.Services.Interfaces
{
    /// <summary>
    /// Interface for services related to transaction history.
    /// </summary>
    public interface ITransactionHistoryService
    {
        /// <summary>
        /// Retrieves transaction histories based on the specified account ID.
        /// </summary>
        /// <param name="toAccountId">Account ID to filter the transaction histories.</param>
        /// <returns>A list of transaction history responses with customer details.</returns>
        IEnumerable<TransactionHistoryResponse> GetTransactionHistoriesByAccount(int toAccountId);

        /// <summary>
        /// Creates a new transaction asynchronously with business logic validation.
        /// </summary>
        /// <param name="transactionRequest">The request containing details of the transaction to create.</param>
        /// <returns>A task that represents the asynchronous operation, with a result indicating success.</returns>
        Task<string> CreateTransactionAsync(TransactionHistoryRequest transactionRequest);
    }
}
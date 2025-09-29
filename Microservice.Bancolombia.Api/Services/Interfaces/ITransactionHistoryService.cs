using Microservice.Bancolombia.Api.Entities.Domain;

namespace Microservice.Bancolombia.Api.Services.Interfaces
{
    /// <summary>
    /// Interface for transaction history service operations.
    /// </summary>
    public interface ITransactionHistoryService
    {
        /// <summary>
        /// Retrieves transaction histories based on the specified account ID with customer details.
        /// </summary>
        /// <param name="toAccountId">Account ID to filter the transaction histories.</param>
        /// <returns>A list of transaction history responses with customer details.</returns>
        IEnumerable<TransactionHistoryResponse> GetTransactionHistoriesByAccount(int toAccountId);

        /// <summary>
        /// Creates a new transaction asynchronously with comprehensive business logic validation.
        /// </summary>
        /// <param name="transactionRequest">The transaction request containing the details of the transaction to create.</param>
        /// <returns>A task representing the asynchronous operation, with a TransactionHistoryResponse containing the transaction details.</returns>
        Task<TransactionHistoryResponse> CreateTransactionAsync(TransactionHistoryRequest transactionRequest);
    }
}
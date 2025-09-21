using Microservice.Bancolombia.Api.Entities.Domain;
using Microservice.Bancolombia.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Bancolombia.Api.Controllers
{
    /// <summary>
    /// Controller for managing transaction history operations.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TransactionHistoryController"/> class.
    /// </remarks>
    /// <param name="transactionHistoryService">The transaction history service instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="transactionHistoryService"/> is null.</exception>
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionHistoryController(ITransactionHistoryService transactionHistoryService) : BaseApiController
    {
        /// <summary>
        /// The service instance for managing transaction histories.
        /// </summary>
        private readonly ITransactionHistoryService _transactionHistoryService = transactionHistoryService ?? throw new ArgumentNullException(nameof(transactionHistoryService));

        /// <summary>
        /// Retrieves transaction history for a specific account.
        /// </summary>
        /// <param name="toAccountId">The ID of the account to retrieve transaction history for.</param>
        /// <returns>An action result containing a collection of transaction history responses.</returns>
        /// <response code="200">Returns the collection of transaction history responses.</response>
        /// <response code="400">If the account ID is invalid.</response>
        /// <response code="404">If the account is not found.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [HttpGet("account/{toAccountId}")]
        [ProducesResponseType(typeof(IEnumerable<TransactionHistoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<TransactionHistoryResponse>> GetTransactionHistoriesByAccount(int toAccountId)
        {
            if (toAccountId <= 0)
            {
                return BadRequest("Account ID must be greater than zero.");
            }

            return this.ExecuteWithErrorHandling(() => this._transactionHistoryService.GetTransactionHistoriesByAccount(toAccountId));
        }

        /// <summary>
        /// Creates a new transaction asynchronously with business logic validation.
        /// </summary>
        /// <param name="transactionRequest">The transaction request containing the transaction information.</param>
        /// <returns>A task representing the asynchronous operation, with an action result containing a string indicating success.</returns>
        /// <response code="201">If the transaction was created successfully.</response>
        /// <response code="400">If a business rule validation fails or request data is invalid.</response>
        /// <response code="404">If the referenced account(s) are not found.</response>
        /// <response code="409">If there's a conflict with the current state of the resource.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CreateTransactionAsync([FromBody] TransactionHistoryRequest transactionRequest)
        {
            if (string.IsNullOrWhiteSpace(transactionRequest.BankCode))
            {
                return BadRequest("Bank code is required.");
            }
            if (transactionRequest.FromAccountId <= 0)
            {
                return BadRequest("From Account ID must be greater than zero.");
            }
            if (transactionRequest.ToAccountId <= 0)
            {
                return BadRequest("To Account ID must be greater than zero.");
            }
            if (string.IsNullOrWhiteSpace(transactionRequest.TransactionType))
            {
                return BadRequest("Transaction type is required.");
            }
            if (transactionRequest.TransactionType.Length != 3)
            {
                return BadRequest("Transaction type must be exactly 3 characters.");
            }
            if (transactionRequest.Amount <= 0)
            {
                return BadRequest("Transaction amount must be greater than zero.");
            }

            string[] validTransactionTypes = new[] { "DEP", "WTH", "TRF" };
            if (!validTransactionTypes.Contains(transactionRequest.TransactionType.ToUpperInvariant()))
            {
                return BadRequest("Transaction type must be DEP (Deposit), WTH (Withdrawal), or TRF (Transfer).");
            }

            ActionResult<string> result = await this.ExecuteWithErrorHandlingAsync(() => this._transactionHistoryService.CreateTransactionAsync(transactionRequest));

            if (result.Result is OkObjectResult okResult)
            {
                return StatusCode(StatusCodes.Status201Created, okResult.Value);
            }

            return result;
        }
    }
}
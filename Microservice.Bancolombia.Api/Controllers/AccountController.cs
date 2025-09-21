using Microservice.Bancolombia.Api.Entities.Domain;
using Microservice.Bancolombia.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Bancolombia.Api.Controllers
{
    /// <summary>
    /// Controller for managing account operations.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </remarks>
    /// <param name="accountService">The account service instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="accountService"/> is null.</exception>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController(IAccountService accountService) : BaseApiController
    {
        /// <summary>
        /// The service instance for managing accounts.
        /// </summary>
        private readonly IAccountService _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));

        /// <summary>
        /// Retrieves a collection of account responses based on an account ID.
        /// </summary>
        /// <param name="accountId">The ID of the account to filter by.</param>
        /// <returns>An action result containing a collection of account responses.</returns>
        /// <response code="200">Returns the collection of account responses.</response>
        /// <response code="400">If a business rule validation fails.</response>
        /// <response code="404">If a requested resource is not found.</response>
        /// <response code="409">If there's a conflict with the current state of the resource.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AccountResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<AccountResponse>> GetAccounts(int? accountId)
        {
            return this.ExecuteWithErrorHandling(() => this._accountService.GetAccounts(accountId));
        }

        /// <summary>
        /// Creates a new account asynchronously.
        /// </summary>
        /// <param name="accountRequest">The account request containing the account information.</param>
        /// <returns>A task representing the asynchronous operation, with an action result containing a string indicating success.</returns>
        /// <response code="201">If the account was created successfully.</response>
        /// <response code="400">If a business rule validation fails or request data is invalid.</response>
        /// <response code="409">If an account with the same customer name already exists.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CreateAccountAsync([FromBody] AccountRequest accountRequest)
        {
            if (string.IsNullOrWhiteSpace(accountRequest.CustomerName))
            {
                return BadRequest("Customer name is required.");
            }
            if (accountRequest.TotalBalance < 0)
            {
                return BadRequest("Total balance cannot be negative.");
            }

            var result = await this.ExecuteWithErrorHandlingAsync(() => this._accountService.CreateAccountAsync(accountRequest));

            if (result.Result is OkObjectResult okResult)
            {
                return StatusCode(StatusCodes.Status201Created, okResult.Value);
            }

            return result;
        }

        /// <summary>
        /// Updates an existing account asynchronously.
        /// </summary>
        /// <param name="accountRequest">The account request containing updated account information.</param>
        /// <returns>A task representing the asynchronous operation, with an action result containing a string indicating success.</returns>
        /// <response code="200">If the account was updated successfully.</response>
        /// <response code="400">If a business rule validation fails or request data is invalid.</response>
        /// <response code="404">If the account to update is not found.</response>
        /// <response code="409">If updating would result in a conflict with existing data.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [HttpPut]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<ActionResult<string>> UpdateAccountAsync([FromBody] AccountRequest accountRequest)
        {
            if (accountRequest.AccountId <= 0)
            {
                return Task.FromResult<ActionResult<string>>(BadRequest("Account ID is required for update operations."));
            }
            if (string.IsNullOrWhiteSpace(accountRequest.CustomerName))
            {
                return Task.FromResult<ActionResult<string>>(BadRequest("Customer name is required."));
            }
            if (accountRequest.TotalBalance < 0)
            {
                return Task.FromResult<ActionResult<string>>(BadRequest("Total balance cannot be negative."));
            }

            return this.ExecuteWithErrorHandlingAsync(() => this._accountService.UpdateAccountAsync(accountRequest));
        }

        /// <summary>
        /// Deletes an existing account asynchronously.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns>A task representing the asynchronous operation, with an action result containing a string indicating success.</returns>
        /// <response code="200">If the account was deleted successfully.</response>
        /// <response code="400">If a business rule validation fails.</response>
        /// <response code="404">If the account to delete is not found.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [HttpDelete("{accountId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<ActionResult<string>> DeleteAccountAsync(int accountId)
        {
            if (accountId <= 0)
            {
                return Task.FromResult<ActionResult<string>>(BadRequest("Account ID must be greater than zero."));
            }

            return this.ExecuteWithErrorHandlingAsync(() => this._accountService.DeleteAccountAsync(accountId));
        }
    }
}
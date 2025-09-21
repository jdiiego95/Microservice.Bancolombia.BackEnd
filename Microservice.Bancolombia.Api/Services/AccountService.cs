using Microservice.Bancolombia.Api.Clients.Interfaces;
using Microservice.Bancolombia.Api.Entities.Domain;
using Microservice.Bancolombia.Api.Entities.Model;
using Microservice.Bancolombia.Api.Exceptions;
using Microservice.Bancolombia.Api.Properties;
using Microservice.Bancolombia.Api.Services.Interfaces;
using System.Globalization;
using System.Linq.Expressions;
using System.Resources;

namespace Microservice.Bancolombia.Api.Services
{
    /// <summary>
    /// Provides services for managing accounts.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AccountService"/> class.
    /// </remarks>
    /// <param name="logger">The logger instance.</param>
    /// <param name="accountClient">The account client instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="accountClient"/> is null.</exception>
    public class AccountService(ILogger<BaseService> logger, IAccountClient accountClient) : BaseService(logger), IAccountService
    {
        /// <summary>
        /// The client used to interact with accounts.
        /// </summary>
        private readonly IAccountClient _accountClient = accountClient ?? throw new ArgumentNullException(nameof(accountClient));

        /// <summary>
        /// Retrieves accounts based on the specified account ID.
        /// </summary>
        /// <param name="accountId">Optional account ID to filter the accounts.</param>
        /// <returns>A list of account responses.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during the operation.</exception>
        public IEnumerable<AccountResponse> GetAccounts(int? accountId)
        {
            try
            {
                Expression<Func<Account, bool>>? filter = accountId > 0 ? x => x.AccountId == accountId : null;

                IEnumerable<Account> accounts = this._accountClient.GetAccounts(filter);

                return GetAccountsResponse(accounts);
            }
            catch (Exception ex)
            {
                this.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Creates a new account asynchronously.
        /// </summary>
        /// <param name="accountRequest">The account request containing the details of the account to create.</param>
        /// <returns>A task representing the asynchronous operation, with a result indicating the success of the operation.</returns>
        /// <exception cref="EntityAlreadyExistsException">Thrown when an account with the same customer name already exists.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during the operation.</exception>
        public async Task<string> CreateAccountAsync(AccountRequest accountRequest)
        {
            try
            {
                Account? currentAccount = this._accountClient.GetAccount(x => x.CustomerName.Equals(accountRequest.CustomerName));

                if (currentAccount == null)
                {
                    Account newAccount = CreateNewAccountFromAccountRequest(accountRequest);

                    await this._accountClient.CreateAccountAsync(newAccount).ConfigureAwait(false);

                    return string.Format(CultureInfo.CurrentCulture, Resources.EntityCreatedSuccessfully, accountRequest.CustomerName);
                }
                else
                {
                    throw new EntityAlreadyExistsException(accountRequest.CustomerName);
                }
            }
            catch (Exception ex)
            {
                this.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Delete an existing account asynchronously.
        /// </summary>
        /// <param name="accountId">Account identifier.</param>
        /// <returns>A task representing the asynchronous operation, with a result indicating the success of the operation.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the account does not exist.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during the operation.</exception>
        public async Task<string> DeleteAccountAsync(int accountId)
        {
            try
            {
                Account? currentAccount = this._accountClient.GetAccount(x => x.AccountId.Equals(accountId));

                if (currentAccount != null)
                {
                    await this._accountClient.DeleteAccountAsync(accountId).ConfigureAwait(false);

                    return Resources.EntityDeleteSuccessfully;
                }
                else
                {
                    throw new EntityNotFoundException(accountId);
                }
            }
            catch (Exception ex)
            {
                this.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates an existing account asynchronously.
        /// </summary>
        /// <param name="accountRequest">The account request containing updated information.</param>
        /// <returns>A task representing the asynchronous operation, with a result indicating the success of the operation.</returns>
        /// <exception cref="EntityNotFoundException">Thrown when the account does not exist.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during the operation.</exception>
        public async Task<string> UpdateAccountAsync(AccountRequest accountRequest)
        {
            try
            {
                Account? currentAccount = this._accountClient.GetAccount(x => x.AccountId == accountRequest.AccountId);

                if (currentAccount == null)
                {
                    throw new EntityNotFoundException(accountRequest.AccountId);
                }

                Account? existingAccountWithSameName = this._accountClient.GetAccount(x =>
                    x.CustomerName == accountRequest.CustomerName &&
                    x.AccountId != accountRequest.AccountId);

                if (existingAccountWithSameName != null)
                {
                    throw new EntityAlreadyExistsException(accountRequest.CustomerName);
                }

                currentAccount.CustomerName = accountRequest.CustomerName;
                currentAccount.TotalBalance = accountRequest.TotalBalance;

                await this._accountClient.UpdateAccountAsync(currentAccount).ConfigureAwait(false);

                return string.Format(CultureInfo.CurrentCulture,
                    Resources.EntityUpdateSuccessfully,
                    accountRequest.CustomerName);
            }
            catch (Exception ex)
            {
                this.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Creates account response objects from a list of accounts.
        /// </summary>
        /// <param name="accounts">The list of accounts.</param>
        /// <returns>A list of account response objects.</returns>
        private static IEnumerable<AccountResponse> GetAccountsResponse(IEnumerable<Account> accounts)
        {
            return from account in accounts
                   select new AccountResponse
                   {
                       AccountId = account.AccountId,
                       CustomerName = account.CustomerName,
                       TotalBalance = account.TotalBalance
                   };
        }

        /// <summary>
        /// Creates an account entity from an account request.
        /// </summary>
        /// <param name="accountRequest">The account request containing the details of the account to create.</param>
        /// <returns>The created account entity.</returns>
        private static Account CreateNewAccountFromAccountRequest(AccountRequest accountRequest)
        {
            return new Account
            {
                AccountId = accountRequest.AccountId,
                CustomerName = accountRequest.CustomerName,
                TotalBalance = accountRequest.TotalBalance
            };
        }
    }
}
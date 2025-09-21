using System.Globalization;
using Microservice.Bancolombia.Api.Clients.Interfaces;
using Microservice.Bancolombia.Api.Entities.Domain;
using Microservice.Bancolombia.Api.Entities.Model;
using Microservice.Bancolombia.Api.Exceptions;
using Microservice.Bancolombia.Api.Properties;
using Microservice.Bancolombia.Api.Services.Interfaces;

namespace Microservice.Bancolombia.Api.Services
{
    /// <summary>
    /// Provides services for managing transaction history with business logic validation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TransactionHistoryService"/> class.
    /// </remarks>
    /// <param name="logger">The logger instance.</param>
    /// <param name="transactionHistoryClient">The transaction history client instance.</param>
    /// <param name="accountClient">The account client instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when any client parameter is null.</exception>
    public class TransactionHistoryService(
        ILogger<BaseService> logger,
        ITransactionHistoryClient transactionHistoryClient,
        IAccountClient accountClient) : BaseService(logger), ITransactionHistoryService
    {
        /// <summary>
        /// The client used to interact with transaction histories.
        /// </summary>
        private readonly ITransactionHistoryClient _transactionHistoryClient = transactionHistoryClient ?? throw new ArgumentNullException(nameof(transactionHistoryClient));

        /// <summary>
        /// The client used to interact with accounts.
        /// </summary>
        private readonly IAccountClient _accountClient = accountClient ?? throw new ArgumentNullException(nameof(accountClient));

        // Constantes para tipos de transacción
        private const string TRANSACTION_TYPE_DEPOSIT = "DEP"; // Ingreso
        private const string TRANSACTION_TYPE_WITHDRAWAL = "WTH"; // Retiro
        private const string TRANSACTION_TYPE_TRANSFER = "TRF"; // Transferencia

        /// <summary>
        /// Retrieves transaction histories based on the specified account ID with customer details.
        /// </summary>
        /// <param name="toAccountId">Account ID to filter the transaction histories.</param>
        /// <returns>A list of transaction history responses with customer details.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during the operation.</exception>
        public IEnumerable<TransactionHistoryResponse> GetTransactionHistoriesByAccount(int toAccountId)
        {
            try
            {
                IEnumerable<TransactionHistory> transactions = this._transactionHistoryClient.GetTransactionHistoriesByToAccountId(toAccountId);

                return GetTransactionHistoriesResponse(transactions);
            }
            catch (Exception ex)
            {
                this.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Creates a new transaction asynchronously with comprehensive business logic validation.
        /// </summary>
        /// <param name="transactionRequest">The transaction request containing the details of the transaction to create.</param>
        /// <returns>A task representing the asynchronous operation, with a result indicating the success of the operation.</returns>
        /// <exception cref="InvalidAccountException">Thrown when the account does not exist.</exception>
        /// <exception cref="InsufficientBalanceException">Thrown when there are insufficient funds for withdrawal.</exception>
        /// <exception cref="SameAccountTransactionException">Thrown when trying to transfer to the same account.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during the operation.</exception>
        public async Task<string> CreateTransactionAsync(TransactionHistoryRequest transactionRequest)
        {
            try
            {
                // Validar que no sean la misma cuenta en transferencias
                if (transactionRequest.TransactionType == TRANSACTION_TYPE_TRANSFER &&
                    transactionRequest.FromAccountId == transactionRequest.ToAccountId)
                {
                    throw new SameAccountTransactionException();
                }

                // Procesar según el tipo de transacción
                switch (transactionRequest.TransactionType)
                {
                    case TRANSACTION_TYPE_DEPOSIT:
                        return await ProcessDepositTransactionAsync(transactionRequest).ConfigureAwait(false);

                    case TRANSACTION_TYPE_WITHDRAWAL:
                        return await ProcessWithdrawalTransactionAsync(transactionRequest).ConfigureAwait(false);

                    case TRANSACTION_TYPE_TRANSFER:
                        return await ProcessTransferTransactionAsync(transactionRequest).ConfigureAwait(false);

                    default:
                        throw new ArgumentException($"Tipo de transacción no válido: {transactionRequest.TransactionType}");
                }
            }
            catch (Exception ex)
            {
                this.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Processes a deposit transaction (ingreso de dinero).
        /// </summary>
        /// <param name="transactionRequest">The transaction request.</param>
        /// <returns>A success message.</returns>
        private async Task<string> ProcessDepositTransactionAsync(TransactionHistoryRequest transactionRequest)
        {
            // Validar que la cuenta de destino exista
            Account? toAccount = this._accountClient.GetAccount(x => x.AccountId == transactionRequest.ToAccountId);

            if (toAccount == null)
            {
                throw new InvalidAccountException($"La cuenta {transactionRequest.ToAccountId} no existe en Bancolombia. Verifica el número de cuenta e intenta nuevamente.");
            }

            // Sumar el monto al balance de la cuenta de destino
            toAccount.TotalBalance += transactionRequest.Amount;
            await this._accountClient.UpdateAccountAsync(toAccount).ConfigureAwait(false);

            // Crear el registro de transacción
            TransactionHistory newTransaction = CreateTransactionFromRequest(transactionRequest);
            TransactionHistory createdTransaction = await this._transactionHistoryClient.CreateTransactionHistoryAsync(newTransaction).ConfigureAwait(false);

            return string.Format(CultureInfo.CurrentCulture,
                "Depósito exitoso. Se han ingresado ${0:N2} a la cuenta {1}. ID de transacción: {2}",
                transactionRequest.Amount,
                transactionRequest.ToAccountId,
                createdTransaction.TransactionId);
        }

        /// <summary>
        /// Processes a withdrawal transaction (retiro de dinero).
        /// </summary>
        /// <param name="transactionRequest">The transaction request.</param>
        /// <returns>A success message.</returns>
        private async Task<string> ProcessWithdrawalTransactionAsync(TransactionHistoryRequest transactionRequest)
        {
            // Validar que la cuenta de origen exista
            Account? fromAccount = this._accountClient.GetAccount(x => x.AccountId == transactionRequest.FromAccountId);

            if (fromAccount == null)
            {
                throw new InvalidAccountException($"La cuenta {transactionRequest.FromAccountId} no existe en Bancolombia. Verifica el número de cuenta e intenta nuevamente.");
            }

            // Validar que tenga fondos suficientes
            if (fromAccount.TotalBalance < transactionRequest.Amount)
            {
                throw new InsufficientBalanceException();
            }

            // Restar el monto del balance de la cuenta de origen
            fromAccount.TotalBalance -= transactionRequest.Amount;
            await this._accountClient.UpdateAccountAsync(fromAccount).ConfigureAwait(false);

            // Crear el registro de transacción
            TransactionHistory newTransaction = CreateTransactionFromRequest(transactionRequest);
            TransactionHistory createdTransaction = await this._transactionHistoryClient.CreateTransactionHistoryAsync(newTransaction).ConfigureAwait(false);

            return string.Format(CultureInfo.CurrentCulture,
                "Retiro exitoso. Se han retirado ${0:N2} de la cuenta {1}. Saldo restante: ${2:N2}. ID de transacción: {3}",
                transactionRequest.Amount,
                transactionRequest.FromAccountId,
                fromAccount.TotalBalance,
                createdTransaction.TransactionId);
        }

        /// <summary>
        /// Processes a transfer transaction (transferencia entre cuentas).
        /// </summary>
        /// <param name="transactionRequest">The transaction request.</param>
        /// <returns>A success message.</returns>
        private async Task<string> ProcessTransferTransactionAsync(TransactionHistoryRequest transactionRequest)
        {
            // Validar que ambas cuentas existan
            Account? fromAccount = this._accountClient.GetAccount(x => x.AccountId == transactionRequest.FromAccountId);
            Account? toAccount = this._accountClient.GetAccount(x => x.AccountId == transactionRequest.ToAccountId);

            if (fromAccount == null)
            {
                throw new InvalidAccountException($"La cuenta de origen {transactionRequest.FromAccountId} no existe en Bancolombia.");
            }

            if (toAccount == null)
            {
                throw new InvalidAccountException($"La cuenta de destino {transactionRequest.ToAccountId} no existe en Bancolombia.");
            }

            // Validar fondos suficientes en cuenta de origen
            if (fromAccount.TotalBalance < transactionRequest.Amount)
            {
                throw new InsufficientBalanceException();
            }

            // Realizar la transferencia
            fromAccount.TotalBalance -= transactionRequest.Amount;
            toAccount.TotalBalance += transactionRequest.Amount;

            // Actualizar ambas cuentas
            await this._accountClient.UpdateAccountAsync(fromAccount).ConfigureAwait(false);
            await this._accountClient.UpdateAccountAsync(toAccount).ConfigureAwait(false);

            // Crear el registro de transacción
            TransactionHistory newTransaction = CreateTransactionFromRequest(transactionRequest);
            TransactionHistory createdTransaction = await this._transactionHistoryClient.CreateTransactionHistoryAsync(newTransaction).ConfigureAwait(false);

            return string.Format(CultureInfo.CurrentCulture,
                "Transferencia exitosa. ${0:N2} transferidos desde la cuenta {1} hacia la cuenta {2}. ID de transacción: {3}",
                transactionRequest.Amount,
                transactionRequest.FromAccountId,
                transactionRequest.ToAccountId,
                createdTransaction.TransactionId);
        }

        /// <summary>
        /// Creates transaction history response objects from a list of transactions.
        /// </summary>
        /// <param name="transactions">The list of transactions.</param>
        /// <returns>A list of transaction history response objects.</returns>
        private static IEnumerable<TransactionHistoryResponse> GetTransactionHistoriesResponse(IEnumerable<TransactionHistory> transactions)
        {
            return from transaction in transactions
                   select new TransactionHistoryResponse
                   {
                       TransactionId = transaction.TransactionId,
                       BankCode = transaction.BankCode,
                       FromAccountId = transaction.FromAccountId,
                       ToAccountId = transaction.ToAccountId,
                       ToAccountCustomerName = transaction.ToAccount?.CustomerName ?? string.Empty,
                       TransactionType = transaction.TransactionType,
                       Amount = transaction.Amount,
                       TransactionDate = transaction.TransactionDate
                   };
        }

        /// <summary>
        /// Creates a transaction history entity from a transaction request.
        /// </summary>
        /// <param name="transactionRequest">The transaction request containing the details of the transaction to create.</param>
        /// <returns>The created transaction history entity.</returns>
        private static TransactionHistory CreateTransactionFromRequest(TransactionHistoryRequest transactionRequest)
        {
            return new TransactionHistory
            {
                BankCode = transactionRequest.BankCode,
                FromAccountId = transactionRequest.FromAccountId,
                ToAccountId = transactionRequest.ToAccountId,
                TransactionType = transactionRequest.TransactionType,
                Amount = transactionRequest.Amount,
                TransactionDate = DateTime.Now
            };
        }
    }
}
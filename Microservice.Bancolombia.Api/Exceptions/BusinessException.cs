using System.Globalization;
using Microservice.Bancolombia.Api.Properties;

namespace Microservice.Bancolombia.Api.Exceptions
{
    /// <summary>
    /// Base class for business-related exceptions in the Bancolombia API.
    /// </summary>
    /// <remarks>
    /// Business exceptions represent expected error conditions that arise from
    /// business rule violations or validation failures.
    /// </remarks>
    public abstract class BusinessException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code associated with this business exception.
        /// </summary>
        /// <value>
        /// An integer representing the appropriate HTTP status code for this exception.
        /// </value>
        public int StatusCode { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        protected BusinessException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        protected BusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when attempting to create an entity that already exists.
    /// </summary>
    public class EntityAlreadyExistsException : BusinessException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="entityName">The name of the entity that already exists.</param>
        public EntityAlreadyExistsException(string entityName)
            : base(string.Format(CultureInfo.CurrentCulture, Resources.EntityAlreadyExists, entityName))
        {
            StatusCode = StatusCodes.Status409Conflict;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="entityName">The name of the entity that already exists.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public EntityAlreadyExistsException(string entityName, Exception innerException)
            : base(string.Format(CultureInfo.CurrentCulture, Resources.EntityAlreadyExists, entityName), innerException)
        {
            StatusCode = StatusCodes.Status409Conflict;
        }
    }

    /// <summary>
    /// Exception thrown when attempting to access an entity that does not exist.
    /// </summary>
    public class EntityNotFoundException : BusinessException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// </summary>
        /// <param name="entityId">The identifier of the entity that was not found.</param>
        public EntityNotFoundException(object entityId)
            : base(string.Format(CultureInfo.CurrentCulture, Resources.EntityNotExists, entityId))
        {
            StatusCode = StatusCodes.Status404NotFound;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// </summary>
        /// <param name="entityId">The identifier of the entity that was not found.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public EntityNotFoundException(object entityId, Exception innerException)
            : base(string.Format(CultureInfo.CurrentCulture, Resources.EntityNotExists, entityId), innerException)
        {
            StatusCode = StatusCodes.Status404NotFound;
        }
    }

    /// <summary>
    /// Exception thrown when attempting to perform a transaction with insufficient balance.
    /// </summary>
    public class InsufficientBalanceException : BusinessException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsufficientBalanceException"/> class.
        /// </summary>
        public InsufficientBalanceException()
            : base(Resources.InsufficientBalance)
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InsufficientBalanceException"/> class.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public InsufficientBalanceException(Exception innerException)
            : base(Resources.InsufficientBalance, innerException)
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    /// <summary>
    /// Exception thrown when attempting to use an invalid account.
    /// </summary>
    public class InvalidAccountException : BusinessException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAccountException"/> class.
        /// </summary>
        /// <param name="accountId">The identifier of the invalid account.</param>
        public InvalidAccountException(object accountId)
            : base(string.Format(CultureInfo.CurrentCulture, Resources.InvalidAccountError, accountId))
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAccountException"/> class.
        /// </summary>
        /// <param name="accountId">The identifier of the invalid account.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public InvalidAccountException(object accountId, Exception innerException)
            : base(string.Format(CultureInfo.CurrentCulture, Resources.InvalidAccountError, accountId), innerException)
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    /// <summary>
    /// Exception thrown when attempting to perform a transaction between the same account.
    /// </summary>
    public class SameAccountTransactionException : BusinessException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SameAccountTransactionException"/> class.
        /// </summary>
        public SameAccountTransactionException()
            : base(Resources.SameAccountTransactionError)
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SameAccountTransactionException"/> class.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public SameAccountTransactionException(Exception innerException)
            : base(Resources.SameAccountTransactionError, innerException)
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    /// <summary>
    /// Exception thrown for general application errors that are not business-related.
    /// </summary>
    /// <remarks>
    /// This exception is typically used for unexpected system errors, infrastructure
    /// failures, or other technical issues that are not part of normal business flow.
    /// </remarks>
    public class GeneralApplicationException : Exception
    {
        /// <summary>
        /// Gets the unique error identifier for tracking purposes.
        /// </summary>
        public string ErrorId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralApplicationException"/> class.
        /// </summary>
        /// <param name="errorId">The unique error identifier for tracking purposes.</param>
        public GeneralApplicationException(string errorId)
            : base(string.Format(CultureInfo.CurrentCulture, Resources.GeneralApplicationError, errorId))
        {
            ErrorId = errorId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralApplicationException"/> class.
        /// </summary>
        /// <param name="errorId">The unique error identifier for tracking purposes.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public GeneralApplicationException(string errorId, Exception innerException)
            : base(string.Format(CultureInfo.CurrentCulture, Resources.GeneralApplicationError, errorId), innerException)
        {
            ErrorId = errorId;
        }
    }

    /// <summary>
    /// Exception thrown when there are issues with database configuration.
    /// </summary>
    public class DatabaseConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConfigurationException"/> class.
        /// </summary>
        public DatabaseConfigurationException()
            : base(Resources.DataBaseConfigurationError)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConfigurationException"/> class.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public DatabaseConfigurationException(Exception innerException)
            : base(Resources.DataBaseConfigurationError, innerException)
        {
        }
    }
}
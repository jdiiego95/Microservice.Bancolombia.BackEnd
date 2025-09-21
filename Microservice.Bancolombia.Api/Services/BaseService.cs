using System.Resources;
using Microservice.Bancolombia.Api.Exceptions;
using Microservice.Bancolombia.Api.Properties;

namespace Microservice.Bancolombia.Api.Services
{
    /// <summary>
    /// Base service class providing common functionality for logging.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="BaseService"/> class.
    /// </remarks>
    /// <param name="logger">The logger instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is null.</exception>
    public class BaseService(ILogger<BaseService> logger)
    {
        /// <summary>
        /// The logger instance used for writing log entries.
        /// </summary>
        protected readonly ILogger<BaseService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Logs an exception with appropriate handling based on its type.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <remarks>
        /// <para>Business exceptions are logged with their original message as they represent expected business rule violations.</para>
        /// <para>Other exceptions are logged with a generated tracking ID to help correlate user reports with log entries.</para>
        /// </remarks>
        protected void LogException(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);
            if (exception is BusinessException)
            {
                this._logger.LogWarning(exception, Resources.BusinessError, exception.Message);
            }
            else
            {
                Guid errorId = Guid.NewGuid();
                this._logger.LogError(exception, Resources.GeneralApplicationError, errorId);
                throw new GeneralApplicationException(errorId.ToString(), exception);
            }
        }
    }
}
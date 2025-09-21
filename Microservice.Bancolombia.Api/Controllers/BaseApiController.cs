using Microservice.Bancolombia.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Bancolombia.Api.Controllers
{
    /// <summary>
    /// Base controller that provides common functionality for all API controllers.
    /// </summary>
    /// <remarks>
    /// This abstract base class implements shared exception handling and response formatting
    /// to ensure consistent behavior across all API endpoints.
    /// </remarks>
    [ApiController]
    public abstract class BaseApiController : Controller
    {
        /// <summary>
        /// Executes an action with consistent exception handling.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>An ActionResult containing the result of the action or an appropriate error response.</returns>
        /// <remarks>
        /// This method provides standardized exception handling:
        /// - BusinessExceptions are returned with their specific status codes
        /// - Other exceptions are returned as 500 Internal Server Error
        /// </remarks>
        protected ActionResult<T> ExecuteWithErrorHandling<T>(Func<T> action)
        {
            try
            {
                T result = action();
                return this.Ok(result);
            }
            catch (BusinessException ex)
            {
                return this.StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Executes an asynchronous action with consistent exception handling.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="action">The asynchronous action to execute.</param>
        /// <returns>A task representing the asynchronous operation, with an ActionResult containing the result or an appropriate error response.</returns>
        /// <remarks>
        /// This method provides standardized exception handling for async operations:
        /// - BusinessExceptions are returned with their specific status codes
        /// - Other exceptions are returned as 500 Internal Server Error
        /// </remarks>
        protected async Task<ActionResult<T>> ExecuteWithErrorHandlingAsync<T>(Func<Task<T>> action)
        {
            try
            {
                T result = await action().ConfigureAwait(false);
                return this.Ok(result);
            }
            catch (BusinessException ex)
            {
                return this.StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
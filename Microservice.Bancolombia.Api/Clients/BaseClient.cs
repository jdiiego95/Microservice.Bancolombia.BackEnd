using Microservice.Bancolombia.Api.Entities;

namespace Microservice.Bancolombia.Api.Clients
{
    /// <summary>
    /// Provides a base implementation for client classes that interact with Entity Framework context
    /// </summary>
    /// <remarks>
    /// This abstract class serves as a base for specific client implementations,
    /// ensuring a consistent approach to database context dependency injection
    /// </remarks>
    /// <param name="context">The Entity Framework context used for database operations</param>
    /// <exception cref="ArgumentNullException">Thrown when the context is null</exception>
    internal abstract class BaseClient(MainContext context)
    {
        /// <summary>
        /// The Entity Framework context used for performing database operations
        /// </summary>
        /// <remarks>
        /// Protected readonly field to allow derived classes to access the database context
        /// Ensures that a valid context is always available
        /// </remarks>
        protected readonly MainContext _context = context ?? throw new ArgumentNullException(nameof(context));
    }
}
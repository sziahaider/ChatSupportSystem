using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Unit-of-work abstraction that groups repository access and change persistence.
    /// </summary>
    /// <remarks>
    /// Implementations should coordinate repositories and provide a single transactional
    /// boundary via <see cref="SaveChangesAsync"/>. The unit-of-work is disposable to allow
    /// implementations to clean up resources (database connections, contexts, etc.).
    /// </remarks>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Repository for managing <see cref="Agent"/> entities.
        /// </summary>
        IRepository<Agent> Agents { get; }

        /// <summary>
        /// Repository for managing <see cref="ChatSession"/> entities.
        /// </summary>
        IRepository<ChatSession> ChatSessions { get; }

        /// <summary>
        /// Persists all pending changes made through the repositories.
        /// </summary>
        /// <returns>
        /// The number of state entries written to the underlying store. Implementations
        /// should ensure this method completes the unit-of-work transaction.
        /// </returns>
        Task<int> SaveChangesAsync();
    }
}

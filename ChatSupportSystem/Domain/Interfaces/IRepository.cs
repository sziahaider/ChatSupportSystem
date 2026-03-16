using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    /// <summary>
    /// Generic repository abstraction for basic persistence operations on aggregate types.
    /// </summary>
    /// <typeparam name="T">The entity type the repository manages.</typeparam>
    /// <remarks>
    /// Implementations should encapsulate data access logic (EF Core, in-memory, etc.).
    /// Prefer returning read-only or immutable collections for enumeration and keep async I/O in mind.
    /// Repositories are typically used behind a unit-of-work to group changes and control transactions.
    /// </remarks>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the entity.</param>
        /// <returns>
        /// The entity if found; otherwise <c>null</c>. The operation is asynchronous and may involve I/O.
        /// </returns>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves all entities managed by the repository.
        /// </summary>
        /// <returns>
        /// A read-only list of entities. Implementations may return a snapshot or a projected collection.
        /// </returns>
        Task<IReadOnlyList<T>> GetAllAsync();

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that completes when the add operation has been acknowledged.</returns>
        Task AddAsync(T entity);

        /// <summary>
        /// Removes an existing entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove(T entity);
    }
}

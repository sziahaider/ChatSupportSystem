using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation that provides basic persistence operations
    /// for an entity type <typeparamref name="T"/> using an EF Core <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="T">The entity type the repository manages.</typeparam>
    /// <remarks>
    /// This class is a lightweight wrapper around <see cref="DbContext.Set{T}"/> and is intended
    /// to be used behind a unit-of-work (for example, a scoped <see cref="DbContext"/>).
    /// Methods perform asynchronous I/O where appropriate to avoid blocking threads.
    /// </remarks>
    public class Repository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// The EF Core <see cref="DbContext"/> used to access the database.
        /// Protected so derived repositories can access the context for advanced queries.
        /// </summary>
        protected readonly DbContext _context;

        /// <summary>
        /// Initializes a new instance of <see cref="Repository{T}"/>.
        /// </summary>
        /// <param name="context">An open <see cref="DbContext"/> instance (typically injected).</param>
        public Repository(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The entity's GUID identifier.</param>
        /// <returns>The entity if found; otherwise <c>null</c>.</returns>
        public async Task<T?> GetByIdAsync(Guid id) =>
            await _context.Set<T>().FindAsync(id);

        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A read-only list containing all entities.</returns>
        public async Task<IReadOnlyList<T>> GetAllAsync() =>
            await _context.Set<T>().ToListAsync();

        /// <summary>
        /// Adds a new entity to the change tracker so it will be inserted on save.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public async Task AddAsync(T entity) =>
            await _context.Set<T>().AddAsync(entity);

        /// <summary>
        /// Marks the specified entity for removal from the database on save.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void Remove(T entity) =>
            _context.Set<T>().Remove(entity);
    }
}

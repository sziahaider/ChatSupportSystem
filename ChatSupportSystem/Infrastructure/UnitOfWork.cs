using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    /// <summary>
    /// Concrete unit-of-work that coordinates repositories and commits changes using <see cref="AppDbContext"/>.
    /// </summary>
    /// <remarks>
    /// - Provides access to repositories for aggregates (Agent, ChatSession).
    /// - Lazily initializes repository instances to avoid unnecessary allocations.
    /// - Exposes <see cref="SaveChangesAsync"/> to persist a transactional batch of changes.
    /// </remarks>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // Backing fields for lazily created repositories.
        private IRepository<Agent>? _agent;
        private IRepository<ChatSession>? _chatsession;

        /// <summary>
        /// Constructs the unit-of-work with the database context used for persistence.
        /// </summary>
        /// <param name="context">The <see cref="AppDbContext"/> instance used to query and save entities.</param>
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Repository for accessing and modifying <see cref="Agent"/> entities.
        /// Lazily created on first access to reduce startup cost.
        /// </summary>
        public IRepository<Agent> Agents => _agent ??= new Repository<Agent>(_context);

        /// <summary>
        /// Repository for accessing and modifying <see cref="ChatSession"/> entities.
        /// Lazily created on first access to reduce startup cost.
        /// </summary>
        public IRepository<ChatSession> ChatSessions => _chatsession ??= new Repository<ChatSession>(_context);

        /// <summary>
        /// Persists all changes made through the repositories to the underlying database.
        /// </summary>
        /// <returns>
        /// The number of state entries written to the underlying store.
        /// </returns>
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        /// <summary>
        /// Disposes the underlying database context and any unmanaged resources.
        /// </summary>
        public void Dispose() => _context.Dispose();
    }
}

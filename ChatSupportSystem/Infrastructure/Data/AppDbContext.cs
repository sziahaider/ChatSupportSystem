using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    /// <summary>
    /// EF Core <see cref="DbContext"/> for the application.
    /// </summary>
    /// <remarks>
    /// Exposes DbSet properties for domain entities and applies entity configurations
    /// discovered in this assembly. Database seeding (for development or integration tests)
    /// can be performed by calling <see cref="SeedData.Initialize"/> during startup after
    /// ensuring the database is created/migrated.
    /// </remarks>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Creates a new <see cref="AppDbContext"/> with the given options.
        /// </summary>
        /// <param name="options">The options used to configure the context (typically injected).</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        /// <summary>
        /// Agents available in the system.
        /// Use LINQ queries against this property to query agent data.
        /// </summary>
        public DbSet<Agent> Agents => Set<Agent>();

        /// <summary>
        /// Chat sessions persisted while clients wait for assignment or are active.
        /// </summary>
        public DbSet<ChatSession> ChatSessions => Set<ChatSession>();

        /// <summary>
        /// Apply entity configurations from the current assembly.
        /// </summary>
        /// <remarks>
        /// This centralizes EF Core configuration discovery so individual IEntityTypeConfiguration
        /// implementations are applied automatically.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

    }
}

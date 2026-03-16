using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Seed
{
    /// <summary>
    /// Provides initial seed data for the application database.
    /// Call <see cref="Initialize"/> during application startup to ensure required
    /// agents exist in the database for development and integration testing scenarios.
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Ensures the database contains a minimal set of agents.
        /// If any agents already exist this method returns immediately.
        /// New agents are created with generated GUIDs and marked as on-shift.
        /// </summary>
        /// <param name="db">The application database context to seed.</param>
        public static void Initialize(AppDbContext db)
        {
            // If the Agents table already has data, do nothing.
            if (db.Agents.Any()) return;

            // Add a small, representative set of agents covering different seniority levels.
            db.Agents.AddRange(

                new Agent { Id = Guid.NewGuid(), Name = "Junior1", Seniority = SeniorityLevel.Junior, IsOnShift = true },
                new Agent { Id = Guid.NewGuid(), Name = "Junior2", Seniority = SeniorityLevel.Junior, IsOnShift = true },
                new Agent { Id = Guid.NewGuid(), Name = "Mid1",    Seniority = SeniorityLevel.Mid,    IsOnShift = true },
                new Agent { Id = Guid.NewGuid(), Name = "Senior1", Seniority = SeniorityLevel.Senior, IsOnShift = true }

            );

            // Persist the seeded data.
            db.SaveChanges();
        }
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    /// <summary>
    /// Configures the EF Core mapping for the <see cref="Agent"/> entity.
    /// Keeps table name, key and individual property mappings centralized for the DbContext.
    /// </summary>
    public class AgentConfiguration : IEntityTypeConfiguration<Agent>
    {
        /// <summary>
        /// Apply configuration for the <see cref="Agent"/> entity.
        /// </summary>
        /// <param name="builder">Entity type builder provided by EF Core.</param>
        public void Configure(EntityTypeBuilder<Agent> builder)
        {
            // Map to table "Agent"
            builder.ToTable("Agent");

            // Primary key
            builder.HasKey(x => x.Id);

            // Properties
            // Human-readable name (limit to 100 chars)
            builder.Property(x => x.Name).HasMaxLength(100);

            // Agent's seniority level (enum)
            builder.Property(x => x.Seniority);

            // Number of chats the agent is currently handling
            builder.Property(x => x.CurrentChats);

            // True when this agent is marked as overflow (accepts extra load)
            builder.Property(x => x.IsOverflow);

            // True when the agent is currently on shift and available
            builder.Property(x => x.IsOnShift);
        }
    }
}

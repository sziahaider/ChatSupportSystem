using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    /// <summary>
    /// EF Core configuration for the <see cref="ChatSession"/> entity.
    /// Configures table mapping, key and individual property mappings.
    /// </summary>
    public class ChatSessionConfiguration : IEntityTypeConfiguration<ChatSession>
    {
        /// <summary>
        /// Configure the entity type builder for <see cref="ChatSession"/>.
        /// Keep configuration minimal here; additional constraints/indices can be added as needed.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ChatSession> builder)
        {
            // Map to table "ChatSession"
            builder.ToTable("ChatSession");

            // Primary key
            builder.HasKey(x => x.Id);

            // Properties
            // Optional assigned agent identifier. Null while waiting in queue.
            // Keep as nullable GUID in the database to represent unassigned sessions.
            builder.Property(x => x.AgentId);

            // UTC timestamp when the session was created.
            // Recommended to store as UTC; ensure any read/write code respects UTC semantics.
            builder.Property(x => x.CreatedAt);

            // Whether the session is currently active (not closed).
            builder.Property(x => x.IsActive);

            // Number of consecutive times the client failed to poll the server.
            // Useful for expiring or cleaning up sessions after repeated missed polls.
            builder.Property(x => x.MissedPolls);
        }
    }
}

namespace Domain.Entities
{
    /// <summary>
    /// Represents a chat session requested by a user.
    /// /// Stored in the persistence layer and tracked while waiting for an agent.
    /// </summary>
    public class ChatSession
    {
        /// <summary>
        /// Unique identifier for this chat session.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// UTC timestamp when the session was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Whether the session is currently active (not closed).
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Number of consecutive times the client failed to poll the server.
        /// Can be used to expire sessions after too many missed polls.
        /// </summary>
        public int MissedPolls { get; set; }

        /// <summary>
        /// Optional assigned agent identifier. Null while waiting in queue.
        /// </summary>
        public Guid? AgentId { get; set; }
    }
}
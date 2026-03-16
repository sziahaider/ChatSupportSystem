using Domain.Enums;

namespace Domain.Entities
{

    /// <summary>
    /// Represents an agent who can handle chats in the system.
    /// </summary>
    public class Agent 
	{
        /// <summary>
        /// Unique identifier for the agent.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Human-readable agent name.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Agent's seniority level used to route or prioritise chats.
        /// Uses <see cref="Domain.Enums.SeniorityLevel"/>.
        /// </summary>
        public SeniorityLevel Seniority { get; set; }

        /// <summary>
        /// Number of chats the agent is currently handling.
        /// Used for load balancing and overflow decisions.
        /// </summary>
        public int CurrentChats { get; set; }

        /// <summary>
        /// True when this agent is marked as overflow (accepts extra load beyond normal capacity).
        /// </summary>
        public bool IsOverflow { get; set; }

        /// <summary>
        /// True when the agent is currently on shift and available to receive new chats.
        /// </summary>
        public bool IsOnShift { get; set; }
    }
}
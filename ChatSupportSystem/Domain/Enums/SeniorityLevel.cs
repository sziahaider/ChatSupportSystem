using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    /// <summary>
    /// Represents an agent's seniority level.
    /// Used to influence routing, capacity calculations and prioritization.
    /// </summary>
    public enum SeniorityLevel
    {
        /// <summary>
        /// Entry-level agent with limited experience.
        /// Typically handles simpler chats and has a lower capacity multiplier.
        /// </summary>
        Junior,

        /// <summary>
        /// Mid-level agent with solid experience.
        /// Balances throughput and complexity handling.
        /// </summary>
        Mid,

        /// <summary>
        /// Senior agent able to handle complex issues and higher responsibility.
        /// </summary>
        Senior,

        /// <summary>
        /// Team lead who may supervise others and handle escalations.
        /// May have a different capacity profile than individual contributors.
        /// </summary>
        TeamLead
    }
}

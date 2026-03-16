using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    /// <summary>
    /// Service responsible for selecting the next available agent to assign an incoming chat.
    /// </summary>
    /// <remarks>
    /// Selection rules implemented by <see cref="GetNextAgent(List{Agent})"/>:
    /// - Only agents currently on shift (<see cref="Agent.IsOnShift"/>) are considered.
    /// - Agents must be under the hard cap of 10 concurrent chats (<see cref="Agent.CurrentChats"/> &lt; 10).
    /// - Preference is given by <see cref="Agent.Seniority"/> (lower enum value first).
    /// - If seniority ties, the agent with fewer <see cref="Agent.CurrentChats"/> is selected.
    /// - Returns null when no eligible agent is found.
    /// </remarks>
    public class AgentAssignmentService
    {
        /// <summary>
        /// Selects the next agent to receive a chat from the provided list.
        /// </summary>
        /// <param name="agents">The list of agents to evaluate. Passing <c>null</c> will result in a <see cref="NullReferenceException"/>.</param>
        /// <returns>
        /// The chosen <see cref="Agent"/> instance according to the selection rules, or <c>null</c> if no agent is eligible.
        /// </returns>
        public Agent? GetNextAgent(List<Agent> agents)
        {
            // Filter to agents who are on shift and below the per-agent chat capacity,
            // then order by seniority and current load to pick the best candidate.
            return agents
                .Where(a => a.IsOnShift && a.CurrentChats < 10)
                .OrderBy(a => a.Seniority)
                .ThenBy(a => a.CurrentChats)
                .FirstOrDefault();
        }
    }
}

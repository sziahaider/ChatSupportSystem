using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    /// <summary>
    /// Application service providing operations for managing agents.
    /// Uses an injected <see cref="IUnitOfWork"/> to persist and query agents.
    /// </summary>
    public class AgentService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructs an <see cref="AgentService"/> with the required unit-of-work.
        /// </summary>
        /// <param name="unitOfWork">The unit-of-work used to access repositories and save changes.</param>
        public AgentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves all agents from the repository.
        /// </summary>
        /// <returns>A list containing all <see cref="Agent"/> instances.</returns>
        public async Task<List<Agent>> GetAllAsync()
        {
            // GetAllAsync returns an enumerable or collection from the repository;
            // convert to a List for callers that expect concrete list semantics.
            return (await _unitOfWork.Agents.GetAllAsync()).ToList();
        }

        /// <summary>
        /// Persists a new agent to the repository and commits the unit of work.
        /// </summary>
        /// <param name="agent">The <see cref="Agent"/> to create.</param>
        /// <returns>The created <see cref="Agent"/> instance (usually with Id populated).</returns>
        public async Task<Agent> CreateAsync(Agent agent)
        {
            await _unitOfWork.Agents.AddAsync(agent);
            await _unitOfWork.SaveChangesAsync();
            return agent;
        }

        /// <summary>
        /// Selects the next available agent to assign an incoming chat.
        /// Selection rules:
        /// - Agent must be on shift (<see cref="Agent.IsOnShift"/> == true).
        /// - Agent must be below the hard cap of 10 concurrent chats (<see cref="Agent.CurrentChats"/> &lt; 10).
        /// - Prefer lower <see cref="Agent.Seniority"/> (enum ordering).
        /// - Break seniority ties by selecting the agent with fewer <see cref="Agent.CurrentChats"/>.
        /// </summary>
        /// <returns>
        /// The selected <see cref="Agent"/>, or <c>null</c> when no eligible agent is found.
        /// </returns>
        public async Task<Agent?> NextAgent()
        {
            var agents = await _unitOfWork.Agents.GetAllAsync();

            // Filter to eligible agents, then order by seniority and current load to choose the best candidate.
            return agents.Where(a => a.IsOnShift && a.CurrentChats < 10)
                         .OrderBy(a => a.Seniority)
                         .ThenBy(a => a.CurrentChats)
                         .FirstOrDefault();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Queue;
using Infrastructure.Time;

namespace Application.Services
{
    /// <summary>
    /// Service that manages chat session lifecycle: creation, polling (keep-alive) and enumeration.
    /// </summary>
    public class ChatSessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly InMemoryQueue _queue;
        private readonly CapacityService _capacityService;
        private readonly AgentService _agentService;
        private readonly IDateTimeProvider _clock;

        /// <summary>
        /// Constructs the <see cref="ChatSessionService"/> with required dependencies.
        /// </summary>
        /// <param name="unitOfWork">Unit of work for persisting chat sessions.</param>
        /// <param name="queue">In-memory queue used to enqueue new session ids.</param>
        /// <param name="capacityService">Service used to compute agent capacity for admission control.</param>
        /// <param name="agentService">Service used to enumerate agents when computing capacity.</param>
        /// <param name="clock">Time provider for testable timestamp generation.</param>
        public ChatSessionService(IUnitOfWork unitOfWork, InMemoryQueue queue, CapacityService capacityService, AgentService agentService, IDateTimeProvider clock)
        {
            _unitOfWork = unitOfWork;
            _queue = queue;
            _capacityService = capacityService;
            _agentService = agentService;
            _clock = clock;
        }

        /// <summary>
        /// Retrieves all persisted chat sessions.
        /// </summary>
        /// <returns>List of all <see cref="ChatSession"/> records.</returns>
        public async Task<List<ChatSession>> GetAllAsync()
        {
            return (await _unitOfWork.ChatSessions.GetAllAsync()).ToList();
        }

        /// <summary>
        /// Handles a poll (keep-alive) from a client for the given session id.
        /// Resets the missed-poll counter so session cleanup logic knows the session is still active.
        /// </summary>
        /// <param name="id">The session id being polled.</param>
        /// <returns><c>true</c> if the session exists and the poll was recorded; otherwise <c>false</c>.</returns>
        public async Task<bool> Poll(Guid id)
        {
            var s = await _unitOfWork.ChatSessions.GetByIdAsync(id);

            if (s == null) return false;

            // Reset missed poll count to indicate the client is still active.
            s.MissedPolls = 0;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }


        /// <summary>
        /// Attempts to create and enqueue a new chat session.
        /// Uses agent capacity to determine a soft maximum queue length and rejects when the queue is above that threshold.
        /// </summary>
        /// <returns>
        /// Tuple where Success indicates whether the session was accepted and SessionId contains the created id or Guid.Empty when rejected.
        /// </returns>
        public async Task<(bool Success, Guid SessionId)> CreateAsync()
        {
            // Get current agents and compute total handling capacity.
            var _agents = await _agentService.GetAllAsync();
            var capacity = _capacityService.Calculate(_agents);

            // Soft queue limit = 150% of capacity to prevent unbounded queue growth.
            int maxQueue = (int)(capacity * 1.5);

            // If the current queue length exceeds the threshold, reject the new incoming chat.
            if (_queue.Length > maxQueue)
                return (false, Guid.Empty);

            // Create a new chat session record and persist it before enqueuing.
            var chatsession = new ChatSession
            {
                Id = Guid.NewGuid(),
                CreatedAt = _clock.UtcNow,
                IsActive = true
            };

            await _unitOfWork.ChatSessions.AddAsync(chatsession);
            await _unitOfWork.SaveChangesAsync();

            // Enqueue the session id so routing/worker logic can pick it up.
            _queue.Enqueue(chatsession.Id);

            return (true, chatsession.Id);
        }
    }
}

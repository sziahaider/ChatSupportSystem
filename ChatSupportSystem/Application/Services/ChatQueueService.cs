using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Application.Services
{
    /// <summary>
    /// Service responsible for managing incoming chat sessions and the queue that backs them.
    /// </summary>
    /// <remarks>
    /// - Uses <see cref="CapacityService"/> to determine current handling capacity from agents.
    /// - Enforces a soft queue limit based on capacity to avoid unbounded queue growth.
    /// - Persists chat sessions through <see cref="IUnitOfWork"/> and enqueues session ids to <see cref="RedisQueueService"/>.
    /// </remarks>
    public class ChatQueueService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RedisQueueService _queue;
        private readonly CapacityService _capacityService;
        private readonly AgentService _agentService;
        private readonly IDateTimeProvider _clock;

        /// <summary>
        /// Constructs the chat queue service with required dependencies.
        /// </summary>
        /// <param name="unitOfWork">Repository/unit-of-work used to persist chat sessions.</param>
        /// <param name="queue">Queue service used to enqueue session identifiers for processing.</param>
        /// <param name="capacityService">Service used to compute total agent capacity.</param>
        /// <param name="agentService">Service used to enumerate agents for capacity calculation.</param>
        /// <param name="clock">Provides current time abstractions for testability.</param>
        public ChatQueueService(IUnitOfWork unitOfWork, RedisQueueService queue, CapacityService capacityService, AgentService agentService, IDateTimeProvider clock)
        {
            _unitOfWork = unitOfWork;
            _queue = queue;
            _capacityService = capacityService;
            _agentService = agentService;
            _clock = clock;
        }
       
        /// <summary>
        /// Returns all persisted chat sessions.
        /// </summary>
        /// <returns>A list containing all <see cref="ChatSession"/> records.</returns>
        public async Task<List<ChatSession>> GetAllAsync()
        {
            return (await _unitOfWork.ChatSessions.GetAllAsync()).ToList();
        }

        /// <summary>
        /// Attempts to create a new chat session and enqueue it for processing.
        /// </summary>
        /// <returns>
        /// A tuple where <c>Success</c> indicates whether the session was accepted;
        /// <c>SessionId</c> contains the created session id when accepted or <see cref="Guid.Empty"/> when rejected.
        /// </returns>
        public async Task<(bool Success, Guid SessionId)> CreateAsync()
        {
            // Get the current list of agents to compute total handling capacity.
            var _agents = await _agentService.GetAllAsync();
            var capacity = _capacityService.Calculate(_agents);

            // Define a soft maximum queue length as 150% of computed capacity to avoid overcommitting.
            int maxQueue = (int)(capacity * 1.5);
            
            // If the queue length already exceeds the threshold, reject the incoming chat.
            if (_queue.Length > maxQueue)
                return (false, Guid.Empty);

            // Create and persist a new chat session record.
            var chatsession = new ChatSession
            {
                Id = Guid.NewGuid(),
                CreatedAt = _clock.UtcNow,
                IsActive = true
            };

            await _unitOfWork.ChatSessions.AddAsync(chatsession);
            await _unitOfWork.SaveChangesAsync();

            // Enqueue the session id so workers or routing logic can pick it up.
            await _queue.Enqueue(chatsession.Id);

            return (true, chatsession.Id);
        }
    }
}

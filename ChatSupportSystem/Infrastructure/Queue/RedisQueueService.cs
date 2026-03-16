using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queue
{
    /// <summary>
    /// Lightweight service-wrapper around <see cref="RedisQueue"/> exposing queue
    /// operations to other application components.
    /// </summary>
    /// <remarks>
    /// This class is intentionally thin — it delegates to <see cref="RedisQueue"/>.
    /// It exists to provide a stable abstraction boundary that can be mocked or replaced
    /// independently of the underlying queue implementation.
    /// </remarks>
    public class RedisQueueService
    {
        // Underlying Redis-backed queue implementation.
        private readonly RedisQueue _queue;

        /// <summary>
        /// Creates a new <see cref="RedisQueueService"/> that forwards operations to <see cref="RedisQueue"/>.
        /// </summary>
        /// <param name="queue">The concrete <see cref="RedisQueue"/> instance to use.</param>
        public RedisQueueService(RedisQueue queue)
        {
            _queue = queue;
        }

        /// <summary>
        /// Enqueues the provided identifier for processing.
        /// </summary>
        /// <param name="id">The identifier to enqueue.</param>
        /// <returns>
        /// A completed <see cref="Task"/>. Note: this method currently does not await the
        /// underlying queue task and returns a completed task immediately — callers should
        /// be aware that the enqueue operation is forwarded but not awaited here.
        /// </returns>
        public Task Enqueue(Guid id)
        {
            // Forward the id to the Redis queue. We intentionally return a completed task
            // to keep this wrapper synchronous from the caller's perspective.
            _queue.Enqueue(id);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Attempts to dequeue the next available identifier.
        /// </summary>
        /// <returns>
        /// A task that resolves to the dequeued <see cref="Guid"/>, or <c>null</c> if the queue is empty.
        /// </returns>
        public async Task<Guid?> Dequeue()
        {
            var id = await _queue.Dequeue();

            // Return the id directly if present; otherwise return null.
            if (id != null)
                return id;

            return null;
        }

        /// <summary>
        /// Gets the current length of the underlying Redis queue.
        /// </summary>
        /// <remarks>
        /// This value is a snapshot and can change immediately after it's read.
        /// </remarks>
        public int Length => _queue.Count;
    }
}

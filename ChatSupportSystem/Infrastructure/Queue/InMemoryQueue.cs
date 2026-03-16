using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;

namespace Infrastructure.Queue
{
    /// <summary>
    /// In-memory, thread-safe implementation of <see cref="IQueue"/> backed by
    /// <see cref="ConcurrentQueue{T}"/>. Suitable for local development and tests.
    /// </summary>
    /// <remarks>
    /// This implementation is fast and thread-safe because it uses <see cref="ConcurrentQueue{T}"/>.
    /// For distributed scenarios or durability, replace with a remote-backed implementation
    /// (for example, Redis or a message broker).
    /// </remarks>
    public class InMemoryQueue : IQueue
    {
        // Underlying thread-safe queue storing session identifiers.
        private readonly ConcurrentQueue<Guid> _queue = new();

        /// <summary>
        /// Enqueues a session identifier for later processing.
        /// This method completes immediately and does not perform I/O.
        /// </summary>
        /// <param name="id">The session <see cref="Guid"/> to enqueue.</param>
        /// <returns>A completed <see cref="Task"/> once the id has been enqueued.</returns>
        public Task Enqueue(Guid id)
        {
            _queue.Enqueue(id);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Attempts to dequeue the next available session identifier.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that resolves to the dequeued <see cref="Guid"/>,
        /// or <c>null</c> if the queue is empty.
        /// </returns>
        public Task<Guid?> Dequeue()
        {
            if (_queue.TryDequeue(out var id))
                return Task.FromResult<Guid?>(id);

            return Task.FromResult<Guid?>(null);
        }

        /// <summary>
        /// Gets the current number of items in the queue.
        /// Note: the count is transient and may change immediately after being read.
        /// </summary>
        public int Length => _queue.Count;
    }
}

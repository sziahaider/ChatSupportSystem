using System;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    /// <summary>
    /// Abstraction for a queue that holds chat session identifiers.
    /// </summary>
    /// <remarks>
    /// Implementations (in-memory, Redis, etc.) back this interface to decouple
    /// producers and consumers of chat sessions and to make the queue replaceable
    /// for testing or scaling.
    /// Implementations should be thread-safe and document any consistency or latency characteristics.
    /// </remarks>
    public interface IQueue
    {
        /// <summary>
        /// Gets the current length of the queue.
        /// </summary>
        /// <remarks>
        /// The value may be approximate or transient for distributed implementations
        /// (for example, a remote Redis-backed queue) and can change immediately after being read.
        /// </remarks>
        int Length { get; }

        /// <summary>
        /// Enqueues a chat session identifier for later processing.
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid"/> of the chat session to enqueue.</param>
        /// <returns>A task that completes when the enqueue operation has been acknowledged.</returns>
        Task Enqueue(Guid sessionId);

        /// <summary>
        /// Dequeues the next available chat session identifier.
        /// </summary>
        /// <returns>
        /// A task that resolves to the dequeued <see cref="Guid"/> when available,
        /// or <c>null</c> if the queue is empty.
        /// </returns>
        Task<Guid?> Dequeue();
    }
}
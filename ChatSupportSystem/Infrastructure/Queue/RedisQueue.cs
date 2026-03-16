using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queue
{
    /// <summary>
    /// Redis-backed queue implementation storing message identifiers in a Redis list.
    /// </summary>
    /// <remarks>
    /// This class uses StackExchange.Redis <see cref="ConnectionMultiplexer"/> and a single
    /// Redis list named "chatQueue" to enqueue (left push) and dequeue (right pop) GUIDs.
    /// The implementation is intentionally simple and intended for development or single-node
    /// queueing scenarios. For production use consider connection pooling, stronger error
    /// handling, and a durable queue design appropriate for your workload.
    /// </remarks>
    public class RedisQueue
    {
        // Connection multiplexer managing connections to Redis.
        private readonly ConnectionMultiplexer _redis;

        // Short-hand reference to the Redis logical database.
        private readonly StackExchange.Redis.IDatabase _db;

        // Configuration settings provided via IOptions<Redis>.
        private readonly Redis _redisSettings;

        /// <summary>
        /// Creates a new <see cref="RedisQueue"/> using the provided Redis settings.
        /// </summary>
        /// <param name="optionsAccessor">Options accessor containing <see cref="Redis"/> settings.</param>
        /// <remarks>
        /// The constructor uses <see cref="ConnectionMultiplexer.Connect(ConfigurationOptions)"/> which
        /// is synchronous and may block during startup. If non-blocking startup is required,
        /// consider using <c>ConnectionMultiplexer.ConnectAsync</c> or initializing the connection
        /// outside of the request path.
        /// </remarks>
        public RedisQueue(IOptions<Redis> optionsAccessor)
        {
            _redisSettings = optionsAccessor.Value;

            var options = new ConfigurationOptions
            {
                EndPoints = { { _redisSettings.Host, _redisSettings.Port } },
                Password = _redisSettings.Password, // Empty string for no password
                Ssl = _redisSettings.Ssl,   // Explicitly enable/disable SSL from settings
                AbortOnConnectFail = false // Optional: set to false to retry connection if it fails initially
            };
            _redis = ConnectionMultiplexer.Connect(options);
            _db = _redis.GetDatabase();
        }

        /// <summary>
        /// Enqueues the specified identifier into the Redis-backed queue.
        /// </summary>
        /// <param name="id">The identifier to enqueue.</param>
        /// <returns>A task that completes once the value has been pushed to Redis.</returns>
        public async Task Enqueue(Guid id)
        {
            // Use a Redis list left-push so that Dequeue can perform a right-pop,
            // creating FIFO semantics across the pair of operations.
            await _db.ListLeftPushAsync("chatQueue", id.ToString());
        }

        /// <summary>
        /// Attempts to dequeue the next identifier from the Redis-backed queue.
        /// </summary>
        /// <returns>
        /// A task resolving to the dequeued <see cref="Guid"/>, or null if the queue is empty.
        /// </returns>
        public async Task<Guid?> Dequeue()
        {
            var v = await _db.ListRightPopAsync("chatQueue");

            if (v.IsNullOrEmpty)
                return null;

            return Guid.Parse(v!);
        }

        /// <summary>
        /// Gets the current number of items in the Redis queue.
        /// </summary>
        /// <remarks>
        /// This property performs a synchronous call to Redis to read the list length.
        /// The returned value is a snapshot and may change immediately after being read.
        /// Consider exposing an asynchronous version if callers require non-blocking behavior.
        /// </remarks>
        public int Count => (int)_db.ListLength("chatQueue");
    }
}

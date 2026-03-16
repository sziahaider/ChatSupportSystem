using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{  
    /// <summary>
    /// Configuration settings for connecting to a Redis server.
    /// </summary>
    /// <remarks>
    /// This POCO is intended to be bound from configuration (for example via <c>IOptions&lt;Redis&gt;</c>).
    /// - Host and Port specify the Redis endpoint.
    /// - Password may be empty or null when no authentication is required.
    /// - Ssl indicates whether to use TLS for the connection.
    /// Validate values during startup when binding from configuration to avoid runtime errors.
    /// </remarks>
    public class Redis
    {
        /// <summary>
        /// Redis host name or IP address.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// TCP port used to connect to Redis.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Password for authenticating with Redis. Empty or null means no password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Whether to enable SSL/TLS when connecting to Redis.
        /// </summary>
        public bool Ssl { get; set; }
    }
}

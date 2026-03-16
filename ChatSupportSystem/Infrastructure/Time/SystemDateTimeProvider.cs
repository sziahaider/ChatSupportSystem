using Domain.Interfaces;
using System;

namespace Infrastructure.Time
{
    /// <summary>
    /// Production implementation of <see cref="IDateTimeProvider"/> that returns the
    /// system UTC clock.
    /// </summary>
    /// <remarks>
    /// Use this provider in production code. For unit tests, inject a deterministic
    /// implementation of <see cref="IDateTimeProvider"/> to avoid depending on the
    /// real system clock.
    /// </remarks>
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        /// <summary>
        /// Gets the current date and time expressed as UTC.
        /// Delegates to <see cref="DateTime.UtcNow"/>.
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
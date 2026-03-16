using System;

namespace Domain.Interfaces
{
    /// <summary>
    /// Abstraction for obtaining the current UTC time.
    /// </summary>
    /// <remarks>
    /// Use this interface to avoid direct calls to <see cref="DateTime.UtcNow"/>,
    /// which improves testability by allowing injection of deterministic time providers.
    /// </remarks>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Gets the current date and time expressed as UTC.
        /// Implementations should return a stable UTC timestamp (e.g. <see cref="DateTime.UtcNow"/>).
        /// </summary>
        DateTime UtcNow { get; }
    }
}
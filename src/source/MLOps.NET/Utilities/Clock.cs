using System;

namespace MLOps.NET.Utilities
{
    /// <summary>
    /// Abstration of DateTime
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// DateTime.UtcNow
        /// </summary>
        DateTime UtcNow { get; }
    }

    internal sealed class Clock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}

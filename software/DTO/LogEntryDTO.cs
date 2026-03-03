using System;

namespace cog1.DTO
{
    /// <summary>
    /// Represents a single in-memory log entry.
    /// </summary>
    public class LogEntryDTO
    {
        /// <summary>
        /// Category of the log entry.
        /// </summary>
        public LogCategory category { get; set; }

        /// <summary>
        /// UTC date/time when the entry was recorded.
        /// </summary>
        public DateTime timestampUtc { get; set; }

        /// <summary>
        /// Severity level of the log entry.
        /// </summary>
        public LogLevel level { get; set; }

        /// <summary>
        /// Log message text.
        /// </summary>
        public string text { get; set; }
    }
}

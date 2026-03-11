using cog1.DTO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cog1.Business
{
    /// <summary>
    /// Business for in-memory logging. Maintains a static, thread-safe list of log entries
    /// limited to 500 entries in total.
    /// </summary>
    public class LoggingBusiness : BusinessBase
    {
        private const int MaxEntries = 500;

        /// <summary>
        /// Combined list of all log entries regardless of category, guarded by <see cref="_lock"/>.
        /// </summary>
        private static readonly List<LogEntryDTO> _entries = new();

        private static readonly object _lock = new();

        public LoggingBusiness(Cog1Context context, ILogger logger) : base(context, logger)
        {
        }

        #region Public methods

        /// <summary>
        /// Enumerates all log categories with their descriptions.
        /// </summary>
        public List<LogCategoryDTO> EnumerateLogCategories()
        {
            var lit = Context.Literals.Logging;
            return Enum.GetValues<LogCategory>()
                .Select(item => new LogCategoryDTO
                {
                    logCategory = item,
                    description = item switch
                    {
                        LogCategory.General => lit.General,
                        LogCategory.Modbus => lit.Modbus,
                        LogCategory.Variables => lit.Variables,
                        LogCategory.Security => lit.Security,
                        LogCategory.Integrations => lit.Integrations,
                        LogCategory.System => lit.System,
                        _ => item.ToString(),
                    },
                })
                .ToList();
        }

        /// <summary>
        /// Enumerates all log levels with their descriptions.
        /// </summary>
        public List<LogLevelDTO> EnumerateLogLevels()
        {
            var lit = Context.Literals.Logging;
            return Enum.GetValues<DTO.LogLevel>()
                .Select(item => new LogLevelDTO
                {
                    logLevel = item,
                    description = item switch
                    {
                        DTO.LogLevel.Information => lit.Information,
                        DTO.LogLevel.Warning => lit.Warning,
                        DTO.LogLevel.Error => lit.Error,
                        _ => item.ToString(),
                    },
                })
                .ToList();
        }

        /// <summary>
        /// Adds a new log entry.
        /// </summary>
        /// <param name="category">Log category.</param>
        /// <param name="level">Severity level.</param>
        /// <param name="text">Log message text.</param>
        public static void Log(LogCategory category, DTO.LogLevel level, string text)
        {
            var entry = new LogEntryDTO
            {
                category = category,
                timestampUtc = DateTime.UtcNow,
                level = level,
                text = text,
            };

            lock (_lock)
            {
                _entries.Add(entry);

                // Trim oldest entries when limit is exceeded
                if (_entries.Count > MaxEntries)
                {
                    _entries.RemoveRange(0, _entries.Count - MaxEntries);
                }
            }
        }

        /// <summary>
        /// Retrieves log entries, optionally filtered by category and/or level.
        /// Returns entries ordered from oldest to newest.
        /// </summary>
        /// <param name="category">If specified, returns only entries for this category.</param>
        /// <param name="level">If specified, returns only entries at or above this level.</param>
        /// <returns>A list of matching log entries.</returns>
        public List<LogEntryDTO> GetEntries(DTO.LogCategory? category = null, DTO.LogLevel? level = null)
        {
            lock (_lock)
            {
                IEnumerable<LogEntryDTO> result = _entries;

                if (category.HasValue)
                {
                    result = result.Where(e => e.category == category.Value);
                }

                if (level.HasValue)
                {
                    result = result.Where(e => e.level == level.Value);
                }

                return result
                    .OrderByDescending(e => e.timestampUtc)
                    .ToList();
            }
        }

        #endregion
    }
}

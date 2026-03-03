using cog1.DTO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cog1.Business
{
    /// <summary>
    /// Business for in-memory logging. Maintains a static, thread-safe list of log entries
    /// limited to 1000 entries per category.
    /// </summary>
    public class LoggingBusiness : BusinessBase
    {
        private const int MaxEntriesPerCategory = 1000;

        /// <summary>
        /// Per-category lists of log entries, guarded by <see cref="_lock"/>.
        /// </summary>
        private static readonly Dictionary<DTO.LogCategory, List<LogEntryDTO>> _entries = new();

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
                if (!_entries.TryGetValue(category, out var list))
                {
                    list = new List<LogEntryDTO>();
                    _entries[category] = list;
                }

                list.Add(entry);

                // Trim oldest entries when limit is exceeded
                if (list.Count > MaxEntriesPerCategory)
                {
                    list.RemoveRange(0, list.Count - MaxEntriesPerCategory);
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
                IEnumerable<LogEntryDTO> result;

                if (category.HasValue)
                {
                    if (_entries.TryGetValue(category.Value, out var list))
                        result = list;
                    else
                        return new List<LogEntryDTO>();
                }
                else
                {
                    result = _entries.Values.SelectMany(l => l);
                }

                if (level.HasValue)
                {
                    result = result.Where(e => e.level >= level.Value);
                }

                return result
                    .OrderBy(e => e.timestampUtc)
                    .ToList();
            }
        }

        #endregion
    }
}

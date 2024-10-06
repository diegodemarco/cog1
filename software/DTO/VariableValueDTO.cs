using cog1.Entities;
using System;

namespace cog1.DTO
{
    public class VariableValueDTO
    {
        /// <summary>
        /// Indicates the unique ID of the variable. 
        /// </summary>
        public int variableId { get; set; }

        /// <summary>
        /// Current value of the variable (null means undefined).
        /// </summary>
        public double? value { get; set; }

        /// <summary>
        /// Current value of the variable (null means undefined).
        /// </summary>
        public DateTime? lastUpdateUtc { get; set; }
    }
}

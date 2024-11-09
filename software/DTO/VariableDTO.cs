using cog1.Entities;

namespace cog1.DTO
{
    public class VariableDTO
    {
        /// <summary>
        /// Indicates the unique ID of the variable. 
        /// Ids 1 thgrough 1000 are reserved for built-in variables.
        /// </summary>
        public int variableId { get; set; }

        /// <summary>
        /// Optional string code assigned to the variable.
        /// </summary>
        public string variableCode { get; set; }

        /// <summary>
        /// Description of the variable.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Units associated to the variable.
        /// </summary>
        public string units { get; set; }

        /// <summary>
        /// Type of variable. See the VariableType enum for more information.
        /// </summary>
        public VariableType type { get; set; }

        /// <summary>
        /// Access type applicable to the variable. See the VariableAccessType 
        /// enum for more information.
        /// </summary>
        public VariableAccessType accessType { get; set; }

        /// <summary>
        /// Indicates the source of the variable See the VariableSource enum
        /// for more information.
        /// </summary>
        public VariableSource source { get; set; }

        /// <summary>
        /// Poll interval, in milliseconds.
        /// - For built-in variables, this represents the interval to poll the 
        ///   hardware of the gateway to read the variable. This only applies to
        ///   input variables (output variables are updated when they change).
        /// - For calculated variables, this indicates the interval to 
        ///   recalculate the value of the variable.
        /// - For Modbus variables, this represents the interval to poll the 
        ///   corresponding register(s) on the bus.
        /// - For external variables, this value is ignored.
        /// </summary>
        public int pollIntervalMs { get; set; }

        /// <summary>
        /// Modbus register information, only valid for variables that have source 
        /// set to "Modbus".
        /// </summary>
        public ModbusRegisterDTO modbusRegister { get; set; }

    }
}

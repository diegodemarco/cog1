namespace cog1.Entities
{
    /// <summary>
    /// Represents the source of a variable.
    /// </summary>
    public enum VariableSource
    {
        /// <summary>
        /// Indicates that the source of the variable is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Indicates that the variable is built-in, i.e. it is 
        /// part of the gateway's hardware (digital input, digital
        /// output, ADC, DAC, etc.)
        /// </summary>
        BuiltIn = 1,

        /// <summary>
        /// Indicates that the variable is calculated based on other 
        /// variables, or via scripting.
        /// </summary>
        Calculated = 2,

        /// <summary>
        /// Indicates that the variable is managed externally, i.e. 
        /// its value is updated via API, not by the gateway itself.
        /// </summary>
        External = 3,

        /// <summary>
        /// Indicates that the variable is read or written to via 
        /// Modbus (RTU or TCP).
        /// </summary>
        Modbus = 4,


    }
}

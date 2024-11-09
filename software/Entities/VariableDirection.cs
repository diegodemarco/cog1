namespace cog1.Entities
{
    /// <summary>
    /// Represents the type of access of a variable.
    /// </summary>
    public enum VariableAccessType
    {
        /// <summary>
        /// Indicates that the direction of the variable is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Indicates that the variable can only be read.
        /// </summary>
        Readonly = 1,

        /// <summary>
        /// Indicates that the variable can be read and written to.
        /// </summary>
        ReadWrite = 2,

        /// <summary>
        /// Indicates that the variable can be read and written to, but
        /// writing to the variable triggers an action such as a write
        /// operation on a Modbus register, a script, etc.
        /// </summary>
        ReadWriteAction = 3,
    }
}

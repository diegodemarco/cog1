namespace cog1.Entities
{
    /// <summary>
    /// Represents the type of a variable
    /// </summary>
    public enum VariableType
    {
        /// <summary>
        /// The type of variable is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Indicates that the variable is binay (0/1).
        /// </summary>
        Binary = 1,

        /// <summary>
        /// Indicates that the variable contains an integer value (64 bits).
        /// </summary>
        Integer = 2,

        /// <summary>
        /// Indicates that the variable contains a floating point value (double precision).
        /// </summary>
        FloatingPoint = 3,
    }
}

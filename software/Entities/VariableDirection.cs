namespace cog1.Entities
{
    /// <summary>
    /// Represents the direction of a variable.
    /// </summary>
    public enum VariableDirection
    {
        /// <summary>
        /// Indicates that the direction of the variable is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Indicates that the variable is read from an input. Input
        /// variables are treated as read/write inside the gateway, 
        /// but are they are not allowed to be changed externally.
        /// </summary>
        Input = 1,

        /// <summary>
        /// Indicates that the variable can be read and written to,
        /// both from inside and outside of the gateway. Writing to
        /// an output variable may trigger an external action such as
        /// writing to a bus.
        /// </summary>
        Output = 2,
    }
}

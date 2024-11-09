namespace cog1.Entities
{
    /// <summary>
    /// Represents the type of a variable
    /// </summary>
    public enum OutputStartupType
    {
        /// <summary>
        /// The type of variable is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The output starts turned off / at its minimum value.
        /// </summary>
        Off = 1,

        /// <summary>
        /// The output starts turnd on / at its maximum value.
        /// </summary>
        On = 2,

        /// <summary>
        /// The output is restored to the last known value.
        /// </summary>
        Restore = 3,
    }
}

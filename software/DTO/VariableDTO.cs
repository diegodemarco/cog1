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
        /// Direction of variable. See the VariableDirection enum for more information.
        /// </summary>
        public VariableDirection direction { get; set; }
        /// <summary>
        /// Indicates if the variable is built in. Built-in variables are part
        /// of the hardware of the controller, and therefore they cannot
        /// be deleted. Other restrictions may apply.
        /// </summary>
        public bool isBuiltIn { get; set; } = false;
    }
}

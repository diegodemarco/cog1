using cog1.Literals;
using System.Collections.Generic;

namespace cog1.DTO
{
    public class BasicEntitiesContainerDTO
    {
        public LiteralsContainerDTO literals { get; set; }
        public List<LocaleDTO> locales { get; set; }
        public List<VariableTypeDTO> variableTypes { get; set; }
        public List<VariableAccessTypeDTO> variableAccessTypes { get; set; }
        public List<VariableSourceDTO> variableSources { get; set; }
        public List<ModbusRegisterTypeDTO> modbusRegisterTypes { get; set; }
        public List<ModbusDataTypeDTO> modbusDataTypes { get; set; }

    }
}

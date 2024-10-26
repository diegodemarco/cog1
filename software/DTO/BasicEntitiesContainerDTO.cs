using cog1.Literals;
using System.Collections.Generic;

namespace cog1.DTO
{
    public class BasicEntitiesContainerDTO
    {
        public LiteralsContainerDTO literals { get; set; }
        public List<LocaleDTO> locales { get; set; }
        public List<VariableTypeDTO> variableTypes { get; set; }
        public List<VariableDirectionDTO> variableDirections { get; set; }

    }
}

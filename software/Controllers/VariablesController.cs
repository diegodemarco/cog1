using cog1.Business;
using cog1.DTO;
using cog1.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace cog1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/variables")]
    public class VariablesController : Cog1ControllerBase
    {
        private readonly ILogger<VariablesController> logger;

        public VariablesController(ILogger<VariablesController> logger, Cog1Context context) : base(context)
        {
            this.logger = logger;
        }

        [HttpGet]
        public List<VariableDTO> GetVariables()
        {
            return MethodPattern(() =>
            {
                return Context.VariableBusiness.EnumerateVariables()
                    .OrderBy(item => item.variableId)
                    .ToList();
            });
        }

        [HttpGet]
        [Route("values")]
        public List<VariableValueDTO> GetVariableValues()
        {
            return MethodPattern(() =>
            {
                return Context.VariableBusiness.GetVariableValues()
                    .OrderBy(item => item.variableId)
                    .ToList();
            });
        }

        [HttpGet]
        [Route("values/{variableId}")]
        public VariableValueDTO GetVariableValues(int variableId)
        {
            return MethodPattern(() =>
            {
                var result = Context.VariableBusiness.GetVariableValues()
                .FirstOrDefault(item => item.variableId == variableId);
                if (result == null)
                    throw new ControllerException(Context.ErrorCodes.Variable.INVALID_VARIABLE_ID);
                return result;
            });
        }

    }
}

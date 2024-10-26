using cog1.Business;
using cog1.DTO;
using cog1.Exceptions;
using cog1.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [Route("{variableId:int}")]
        public VariableDTO GetVariablebyId(int variableId)
        {
            return MethodPattern(() =>
            {
                return Context.VariableBusiness.GetVariable(variableId);
            });
        }

        [HttpPost]
        [RequiresAdmin]
        public VariableDTO CreateVariable([FromBody] VariableDTO v)
        {
            return MethodPattern(() =>
            {
                return Context.VariableBusiness.CreateVariable(v);
            });
        }

        [HttpPut]
        [RequiresAdmin]
        public VariableDTO EditVariable([FromBody] VariableDTO v)
        {
            return MethodPattern(() =>
            {
                return Context.VariableBusiness.EditVariable(v);
            });
        }

        [HttpDelete]
        [RequiresAdmin]
        [Route("{variableId:int}")]
        public void DeleteVariable(int variableId)
        {
            MethodPattern(() =>
            {
                Context.VariableBusiness.DeleteVariable(variableId);
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
        public VariableValueDTO GetVariableValue(int variableId)
        {
            return MethodPattern(() => Context.VariableBusiness.GetVariableValue(variableId));
        }

        [HttpPost]
        [RequiresOperator]
        [Route("values/{variableId}")]
        public VariableValueDTO SetVariableValue(int variableId, [FromBody] double value)
        {
            return MethodPattern(() =>
            {
                Context.VariableBusiness.SetVariableValue(variableId, value);
                return Context.VariableBusiness.GetVariableValue(variableId);
            });
        }

    }
}

using cog1.Business;
using cog1.DTO;
using cog1.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace cog1.Controllers
{
    /*

    [Authorize]
    [ApiController]
    [Route("api/modbus")]
    public class ModbusController : Cog1ControllerBase
    {
        private readonly ILogger<ModbusController> logger;

        public ModbusController(ILogger<ModbusController> logger, Cog1Context context) : base(context)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("registers")]
        public List<ModbusRegisterDTO> EnumerateRegisters()
        {
            return MethodPattern(() =>
            {
                return Context.ModbusBusiness.EnumerateRegisters()
                    .OrderBy(item => item.modbusRegisterId)
                    .ToList();
            });
        }

        [HttpGet]
        [Route("registers/{registerId:int}")]
        public ModbusRegisterDTO GetRegisterById(int registerId)
        {
            return MethodPattern(() =>
            {
                return Context.ModbusBusiness.GetRegister(registerId);
            });
        }

        [HttpPost]
        [RequiresAdmin]
        [Route("registers")]
        public ModbusRegisterDTO CreateRegister([FromBody] ModbusRegisterDTO r)
        {
            return MethodPattern(() =>
            {
                return Context.ModbusBusiness.CreateRegister(r);
            });
        }

        [HttpPut]
        [RequiresAdmin]
        [Route("registers")]
        public ModbusRegisterDTO EditRegister([FromBody] ModbusRegisterDTO r)
        {
            return MethodPattern(() =>
            {
                return Context.ModbusBusiness.EditRegister(r);
            });
        }

        [HttpDelete]
        [RequiresAdmin]
        [Route("registers/{registerId:int}")]
        public void DeleteRegister(int registerId)
        {
            MethodPattern(() =>
            {
                Context.ModbusBusiness.DeleteRegister(registerId);
            });
        }

    }

    */
}

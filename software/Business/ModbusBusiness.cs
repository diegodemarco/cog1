using cog1.DTO;
using cog1.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cog1.Business
{
    /*

    /// <summary>
    /// Business to manage modbus registers
    /// </summary>
    public class ModbusBusiness : BusinessBase
    {
        public ModbusBusiness(Cog1Context context, ILogger logger) : base(context, logger)
        {

        }

        #region CRUD

        public ModbusRegisterDTO CreateRegister(ModbusRegisterDTO r)
        {
            // Fixes
            r.modbusRegisterId = 0;
            // Validations
            ValidateRegister(r);
            return Context.ModbusDao.CreateRegister(r);
        }

        public ModbusRegisterDTO EditRegister(ModbusRegisterDTO v)
        {
            // Make sure Register exists, and validate data
            GetRegister(v.modbusRegisterId);
            ValidateRegister(v);
            return Context.ModbusDao.EditRegister(v);
        }

        public void DeleteRegister(int registerId)
        {
            // Make sure Register exists
            GetRegister(registerId);
            Context.ModbusDao.DeleteRegister(registerId);
        }

        #endregion

    }

    */
}

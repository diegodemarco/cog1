using cog1.Dao;
using cog1.DTO;
using cog1.Exceptions;
using cog1.Hardware;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cog1.Business
{
    /// <summary>
    /// Business to manage variables
    /// </summary>
    public class VariableBusiness : BusinessBase
    {

        public VariableBusiness(Cog1Context context, ILogger logger) : base(context, logger)
        {

        }

        #region private


        #endregion

        #region CRUD

        public bool TryGetVariable(int VariableId, out VariableDTO Variable)
        {
            Variable = Context.VariableDao.GetVariable(VariableId);
            return Variable != null;
        }

        public VariableDTO GetVariable(int VariableId)
        {
            var result = Context.VariableDao.GetVariable(VariableId);
            if (result == null)
                throw new ControllerException(Context.ErrorCodes.Variable.INVALID_VARIABLE_ID);
            return result;
        }

        public List<VariableDTO> EnumerateVariables()
        {
            return Context.VariableDao.EnumerateVariables();
        }

        #endregion

        #region Variable values

        public List<VariableValueDTO> GetVariableValues()
        {
            var result = Context.VariableDao.GetVariableValues();

            // Update with current values for built-in variables
            IOManager.ReadDI(out var di1, out var di2, out var di3, out var di4);
            IOManager.ReadDO(out var do1, out var do2, out var do3, out var do4);
            IOManager.Read010V(out var anv1, out var anv2, out var anv3, out var anv4);
            IOManager.Read020mA(out var ana1, out var ana2, out var ana3, out var ana4);
            foreach (var v in result)
            {
                switch (v.variableId)
                {
                    case  1: v.value = di1 ? 1 : 0; break;
                    case  2: v.value = di2 ? 1 : 0; break;
                    case  3: v.value = di3 ? 1 : 0; break;
                    case  4: v.value = di4 ? 1 : 0; break;
                    case  5: v.value = anv1; break;
                    case  6: v.value = anv2; break;
                    case  7: v.value = anv3; break;
                    case  8: v.value = anv4; break;
                    case  9: v.value = ana1; break;
                    case 10: v.value = ana2; break;
                    case 11: v.value = ana3; break;
                    case 12: v.value = ana4; break;
                    case 13: v.value = do1 ? 1 : 0; break;
                    case 14: v.value = do2 ? 1 : 0; break;
                    case 15: v.value = do3 ? 1 : 0; break;
                    case 16: v.value = do4 ? 1 : 0; break;
                }
            }

            return result;
        }

        #endregion

        #region Startup fixes and housekeeping

        public override void DoStartupFixes()
        {
            Context.VariableDao.DoStartupFixes();
        }

        #endregion

    }
}

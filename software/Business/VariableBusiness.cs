using cog1.DTO;
using cog1.Entities;
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

        #region Basic entities

        public List<VariableTypeDTO> EnumerateVariableTypes()
        {
            return Enum.GetValues<VariableType>()
                .Where(item => item != VariableType.Unknown)
                .Select(item =>
                    new VariableTypeDTO()
                    {
                        variableType = item,
                        description = GetVariableTypeDescription(item)
                    }
                )
                .ToList();
        }

        private string GetVariableTypeDescription(VariableType vt)
        {
            switch (vt)
            {
                case VariableType.Unknown:
                    return "Unknown";
                case VariableType.Integer:
                    return Context.Literals.Variables.Integer;
                case VariableType.FloatingPoint:
                    return Context.Literals.Variables.FLoatingPoint;
                case VariableType.Binary:
                    return Context.Literals.Variables.Binary;
                default:
                    return $"Unknown variable type \"{vt.ToString()}\"";
            }
        }

        private bool IsValidVariableType(VariableType vt)
        {
            return Enum.GetValues<VariableType>()
                .Where(item => item != VariableType.Unknown)
                .ToList()
                .Contains(vt);
        }

        public List<VariableDirectionDTO> EnumerateVariableDirections()
        {
            return Enum.GetValues<VariableDirection>()
                .Where(item => item != VariableDirection.Unknown)
                .Select(item =>
                    new VariableDirectionDTO()
                    {
                        variableDirection = item,
                        description = GetVariableDirectionDescription(item)
                    }
                )
                .ToList();
        }

        private string GetVariableDirectionDescription(VariableDirection vd)
        {
            switch (vd)
            {
                case VariableDirection.Unknown:
                    return "Unknown";
                case VariableDirection.Input:
                    return Context.Literals.Variables.Input;
                case VariableDirection.Output:
                    return Context.Literals.Variables.Output;
                default:
                    return $"Unknown variable direction \"{vd.ToString()}\"";
            }
        }

        private bool IsValidVariableDirection(VariableDirection vd)
        {
            return Enum.GetValues<VariableDirection>()
                .Where(item => item != VariableDirection.Unknown)
                .ToList()
                .Contains(vd);
        }

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
                throw new ControllerException(Context.ErrorCodes.Variables.INVALID_VARIABLE_ID);
            return result;
        }

        public List<VariableDTO> EnumerateVariables()
        {
            return Context.VariableDao.EnumerateVariables();
        }

        public VariableDTO GetVariableByCode(string variableCode)
        {
            var result = Context.VariableDao.GetVariableByCode(variableCode);
            if (result == null)
                throw new ControllerException(Context.ErrorCodes.Variables.INVALID_VARIABLE_CODE);
            return result;
        }

        private void ValidateVariable(VariableDTO v)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(v.description))
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Common.Description));
            if (!IsValidVariableType(v.type))
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Variables.VariableType));
            if (!IsValidVariableDirection(v.direction))
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Variables.VariableDirection));
        }

        public VariableDTO CreateVariable(VariableDTO v)
        {
            v.variableId = 0;
            ValidateVariable(v);
            return Context.VariableDao.CreateVariable(v);
        }

        public VariableDTO EditVariable(VariableDTO v)
        {
            // Make sure variable exists, and validate data
            GetVariable(v.variableId);
            ValidateVariable(v);
            return Context.VariableDao.EditVariable(v);
        }

        public void DeleteVariable(int variableId)
        {
            // Make sure variable exists
            GetVariable(variableId);
            Context.VariableDao.DeleteVariable(variableId);
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

        public VariableValueDTO GetVariableValue(int variableId)
        {
            var result = GetVariableValues()
            .FirstOrDefault(item => item.variableId == variableId);
            if (result == null)
                throw new ControllerException(Context.ErrorCodes.Variables.INVALID_VARIABLE_ID);
            return result;
        }

        public void SetVariableValue(int variableId, double value)
        {
            var v = GetVariable(variableId);
            if (v.isBuiltIn)
            {
                if (v.direction != VariableDirection.Output)
                    throw new ControllerException(Context.ErrorCodes.Variables.VARIABLE_NOT_WRITABLE);
                switch (variableId)
                {
                    case 13:
                        IOManager.SetDigitalOutput(1, value != 0);
                        break;
                    case 14:
                        IOManager.SetDigitalOutput(2, value != 0);
                        break;
                    case 15:
                        IOManager.SetDigitalOutput(3, value != 0);
                        break;
                    case 16:
                        IOManager.SetDigitalOutput(4, value != 0);
                        break;
                    default:
                        throw new ControllerException(Context.ErrorCodes.Variables.VARIABLE_NOT_WRITABLE);
                }
            }
            else
            {
                throw new ControllerException(Context.ErrorCodes.Variables.VARIABLE_NOT_WRITABLE);
            }
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

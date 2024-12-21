using cog1.BackgroundServices;
using cog1.DTO;
using cog1.Entities;
using cog1.Exceptions;
using cog1.Hardware;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
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

        private static Dictionary<ModbusRegisterType, HashSet<ModbusDataType>> registerTypeDataTypes = new()
        {
            { ModbusRegisterType.Coil, new() { ModbusDataType.Boolean } },
            { ModbusRegisterType.DiscreteInput, new() { ModbusDataType.Boolean } },
            { ModbusRegisterType.HoldingRegister, new() { ModbusDataType.UInt16, ModbusDataType.Int16, ModbusDataType.UInt32, ModbusDataType.Int32, ModbusDataType.Float32 } },
            { ModbusRegisterType.InputRegister, new() { ModbusDataType.UInt16, ModbusDataType.Int16, ModbusDataType.UInt32, ModbusDataType.Int32, ModbusDataType.Float32 } },
        };

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
                    return $"Unknown variable type \"{vt}\"";
            }
        }

        private bool IsValidVariableType(VariableType vt)
        {
            return Enum.GetValues<VariableType>()
                .Where(item => item != VariableType.Unknown)
                .Any(item => item == vt);
        }

        public List<VariableAccessTypeDTO> EnumerateVariableAccessTypes()
        {
            return Enum.GetValues<VariableAccessType>()
                .Where(item => item != VariableAccessType.Unknown)
                .Select(item =>
                    new VariableAccessTypeDTO()
                    {
                        accessType = item,
                        description = GetVariableAccessTypeDescription(item)
                    }
                )
                .ToList();
        }

        private string GetVariableAccessTypeDescription(VariableAccessType vd)
        {
            switch (vd)
            {
                case VariableAccessType.Unknown:
                    return "Unknown";
                case VariableAccessType.Readonly:
                    return Context.Literals.Variables.Readonly;
                case VariableAccessType.ReadWrite:
                    return Context.Literals.Variables.ReadWrite;
                case VariableAccessType.ReadWriteAction:
                    return Context.Literals.Variables.ReadWriteAction;
                default:
                    return $"Unknown variable access type \"{vd.ToString()}\"";
            }
        }

        private bool IsValidVariableAccessType(VariableAccessType vd)
        {
            return Enum.GetValues<VariableAccessType>()
                .Where(item => item != VariableAccessType.Unknown)
                .Any(item => item == vd);
        }

        public List<VariableSourceDTO> EnumerateVariableSources()
        {
            return Enum.GetValues<VariableSource>()
                .Where(item => item != VariableSource.Unknown)
                .Select(item =>
                    new VariableSourceDTO()
                    {
                        variableSource = item,
                        description = GetVariableSourceDescription(item)
                    }
                )
                .ToList();
        }

        private string GetVariableSourceDescription(VariableSource vd)
        {
            switch (vd)
            {
                case VariableSource.Unknown:
                    return "Unknown";
                case VariableSource.BuiltIn:
                    return Context.Literals.Variables.BuiltIn;
                case VariableSource.Calculated:
                    return Context.Literals.Variables.Calculated;
                case VariableSource.External:
                    return Context.Literals.Variables.External;
                case VariableSource.Modbus:
                    return Context.Literals.Variables.Modbus;
                default:
                    return $"Unknown variable Source type \"{vd.ToString()}\"";
            }
        }

        private bool IsValidVariableSource(VariableSource vd)
        {
            return Enum.GetValues<VariableSource>()
                .Where(item => item != VariableSource.Unknown)
                .Any(item => item == vd);
        }

        #endregion

        #region Basic entities - Modbus

        public List<ModbusRegisterTypeDTO> EnumerateModbusRegisterTypes()
        {
            return Enum.GetValues<ModbusRegisterType>()
                .Where(item => item != ModbusRegisterType.Unknown)
                .Select(item =>
                    new ModbusRegisterTypeDTO()
                    {
                        modbusRegisterType = item,
                        description = GetModbusRegisterTypeDescription(item)
                    }
                )
                .ToList();
        }

        private string GetModbusRegisterTypeDescription(ModbusRegisterType rt)
        {
            return rt switch
            {
                ModbusRegisterType.Unknown => "Unknown",
                ModbusRegisterType.Coil => Context.Literals.Modbus.Coil,
                ModbusRegisterType.DiscreteInput => Context.Literals.Modbus.DiscreteInput,
                ModbusRegisterType.HoldingRegister => Context.Literals.Modbus.HoldingRegister,
                ModbusRegisterType.InputRegister => Context.Literals.Modbus.InputRegister,
                _ => $"Unknown modbus register type \"{rt.ToString()}\"",
            };
        }

        private bool IsValidModbusRegisterType(ModbusRegisterType rt)
        {
            return Enum.GetValues<ModbusRegisterType>()
                .Where(item => item != ModbusRegisterType.Unknown)
                .Any(item => item == rt);
        }

        public List<ModbusDataTypeDTO> EnumerateModbusDataTypes()
        {
            return Enum.GetValues<ModbusDataType>()
                .Where(item => item != ModbusDataType.Unknown)
                .Select(item =>
                    new ModbusDataTypeDTO()
                    {
                        modbusDataType = item,
                        description = GetModbusDataTypeDescription(item)
                    }
                )
                .ToList();
        }

        private string GetModbusDataTypeDescription(ModbusDataType vd)
        {
            return vd switch
            {
                ModbusDataType.Unknown => "Unknown",
                ModbusDataType.Boolean => Context.Literals.Modbus.DataTypeBoolean,
                ModbusDataType.UInt16 => "UInt-16",
                ModbusDataType.UInt32 => "UInt-32",
                ModbusDataType.Int16 => "Int-16",
                ModbusDataType.Int32 => "Int-32",
                ModbusDataType.Float32 => "Float-32",
                _ => $"Unknown Modbus data type \"{vd.ToString()}\"",
            };
        }

        private bool IsValidModbusDataType(ModbusDataType vd)
        {
            return Enum.GetValues<ModbusDataType>()
                .Where(item => item != ModbusDataType.Unknown)
                .ToList()
                .Contains(vd);
        }

        private bool IsValidModbusTcpHost(ref string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                host = string.Empty;
                return true;
            }
            var parts = host.Split(':');
            if (parts.Length > 2)
                return false;
            if (parts.Length > 1)
            {
                if (!UInt16.TryParse(parts[1], out _))
                    return false;
                host = parts[0].Trim() + ":" + parts[1].Trim();
            }
            else
            {
                host = parts[0].Trim();
            }
            return true;
        }

        private bool IsValidModbusSlaveId(int slaveId)
        {
            return (slaveId >= 1 && slaveId <= 247);
        }

        private bool IsValidModbusRegisterAddress(int registerAddress)
        {
            return (registerAddress >= 1 && registerAddress <= 65535);
        }

        #endregion

        #region CRUD

        public List<VariableDTO> EnumerateVariables()
        {
            return Context.VariableDao.EnumerateVariables();
        }

        public bool TryGetVariable(int variableId, out VariableDTO variable)
        {
            variable = Context.VariableDao.GetVariable(variableId);
            return variable != null;
        }

        public VariableDTO GetVariable(int variableId)
        {
            var result = Context.VariableDao.GetVariable(variableId);
            if (result == null)
                throw new ControllerException(Context.ErrorCodes.Variables.INVALID_VARIABLE_ID);
            return result;
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
            // Fixes
            if (v.source == VariableSource.External)
                v.pollIntervalMs = 0;

            // Validations
            if (string.IsNullOrWhiteSpace(v.description))
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Common.Description));
            if (!IsValidVariableType(v.type))
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Variables.VariableType));
            if (!IsValidVariableAccessType(v.accessType))
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Variables.VariableAccessType));
            if (!IsValidVariableSource(v.source))
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Variables.VariableSource));
            if (v.source == VariableSource.Calculated || v.source == VariableSource.Modbus)
            {
                if (v.pollIntervalMs < 1)
                    throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Variables.PollInterval));
            }

            // Modbus-specific validations
            if (v.source == VariableSource.Modbus)
            {
                // General
                if (v.modbusRegister == null)
                    throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA("modbusRegister"));

                // TCP host
                var host = v.modbusRegister.tcpHost;
                if (!IsValidModbusTcpHost(ref host))
                    throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Modbus.DataType));
                v.modbusRegister.tcpHost = host;

                // Slave ID
                if (!IsValidModbusSlaveId(v.modbusRegister.slaveId))
                    throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Variables.Modbus + " - " + Context.Literals.Modbus.SlaveId));

                // Register address
                if (!IsValidModbusRegisterAddress(v.modbusRegister.registerAddress))
                    throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Variables.Modbus + " - " + Context.Literals.Modbus.RegisterAddress));

                // Register type
                if (!IsValidModbusRegisterType(v.modbusRegister.registerType))
                    throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Variables.Modbus + " - " + Context.Literals.Modbus.RegisterType));

                // Data type
                if (!IsValidModbusDataType(v.modbusRegister.dataType))
                    throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Modbus.DataType));

                // Verify the validity of the combination of the register type and the variable type
                //if (!registerTypeDataTypes[v.modbusRegister.registerType].Contains(v.modbusRegister.dataType))
                //    throw new ControllerException(Context.ErrorCodes.Modbus.INVALID_DATA_TYPE_FOR_REGISTER_TYPE);

                // Verify the validity of the combination of the register type and the data type
                if (!registerTypeDataTypes[v.modbusRegister.registerType].Contains(v.modbusRegister.dataType))
                    throw new ControllerException(Context.ErrorCodes.Modbus.INVALID_DATA_TYPE_FOR_REGISTER_TYPE);
            }
        }

        public VariableDTO CreateVariable(VariableDTO v)
        {
            // Fixes
            v.variableId = 0;
            // Validations
            if (v.source == VariableSource.BuiltIn)
                throw new ControllerException(Context.ErrorCodes.General.INVALID_PARAMETER_VALUE(Context.Literals.Variables.VariableSource, v.source.ToString()));
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
            return Context.VariableDao.GetVariableValues();
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
            if (v.source == VariableSource.BuiltIn)
            {

                // Built-in variables
                if (v.accessType != VariableAccessType.ReadWrite && v.accessType != VariableAccessType.ReadWriteAction)
                    throw new ControllerException(Context.ErrorCodes.Variables.VARIABLE_NOT_WRITABLE);

                if (!IOManager.SetVariableValue(variableId, value))
                    throw new ControllerException(Context.ErrorCodes.Variables.VARIABLE_NOT_WRITABLE);

            }
            else if (v.source == VariableSource.Modbus)
            {

                // Modbus variables
                Console.WriteLine($"Writing register of var {v.variableId} with value {value}");
                if (!ModbusService.WriteRegister(v, value, out string errorMessage))
                    throw new ControllerException(Context.ErrorCodes.Modbus.COULD_NOT_WRITE_REGISTER(errorMessage));

            }
            else
            {

                // Unsupported variable sources
                throw new ControllerException(Context.ErrorCodes.Variables.VARIABLE_NOT_WRITABLE);

            }
        }

        #endregion

        #region Modbus

        #endregion

        #region Startup fixes and housekeeping

        public override void DoStartupFixes()
        {
            Context.VariableDao.DoStartupFixes();
        }

        #endregion

    }
}

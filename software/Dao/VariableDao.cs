using System.Collections.Generic;
using System.Data;
using cog1.Business;
using cog1.DTO;
using System.Linq;
using cog1.Entities;
using Microsoft.Extensions.Logging;
using System;
using cog1.Exceptions;
using cog1.Hardware;
using static cog1.Literals.ModbusLiterals;

namespace cog1.Dao
{
    /// <summary>
    /// Dao class for handling the "Variable" entity
    /// </summary>
    public class VariableDao : DaoBase
    {
        public class BasicVariableDefinition
        {
            public int variableId;
            public VariableType type;
            public VariableAccessType accessType;
            public VariableSource source;
            public int pollIntervalMs;
            public ModbusRegisterDTO modbusRegister;
        }

        private static object _lock = new object();
        private static long variablesSignature = 1;
        private static Dictionary<int, VariableDTO> variables = null;

        public VariableDao(Cog1Context context, ILogger logger) : base(context, logger)
        {
        }

        #region Private methods

        private static List<VariableDTO> EnumerateBuiltInVariables()
        {
            var result = new List<VariableDTO>()
            {
                new VariableDTO() { variableId = IOManager.DI1_VARIABLE_ID, description = "Digital input 1", 
                    type = VariableType.Binary, accessType = VariableAccessType.Readonly },
                new VariableDTO() { variableId = IOManager.DI2_VARIABLE_ID, description = "Digital input 2", 
                    type = VariableType.Binary, accessType = VariableAccessType.Readonly },
                new VariableDTO() { variableId = IOManager.DI3_VARIABLE_ID, description = "Digital input 3", 
                    type = VariableType.Binary, accessType = VariableAccessType.Readonly },
                new VariableDTO() { variableId = IOManager.DI4_VARIABLE_ID, description = "Digital input 4", 
                    type = VariableType.Binary, accessType = VariableAccessType.Readonly },
                new VariableDTO() { variableId = IOManager.AV1_VARIABLE_ID, description = "Voltage input 1", 
                    type = VariableType.FloatingPoint, accessType = VariableAccessType.Readonly, pollIntervalMs = 60000, units = "V" },
                new VariableDTO() { variableId = IOManager.AV2_VARIABLE_ID, description = "Voltage input 2", 
                    type = VariableType.FloatingPoint, accessType = VariableAccessType.Readonly, pollIntervalMs = 60000, units = "V" },
                new VariableDTO() { variableId = IOManager.AV3_VARIABLE_ID, description = "Voltage input 3", 
                    type = VariableType.FloatingPoint, accessType = VariableAccessType.Readonly, pollIntervalMs = 60000, units = "V" },
                new VariableDTO() { variableId = IOManager.AV4_VARIABLE_ID, description = "Voltage input 4", 
                    type = VariableType.FloatingPoint, accessType = VariableAccessType.Readonly, pollIntervalMs = 60000, units = "V" },
                new VariableDTO() { variableId = IOManager.AC1_VARIABLE_ID, description = "Current input 1", 
                    type = VariableType.FloatingPoint, accessType = VariableAccessType.Readonly, pollIntervalMs = 60000, units = "mA" },
                new VariableDTO() { variableId = IOManager.AC2_VARIABLE_ID, description = "Current input 2", 
                    type = VariableType.FloatingPoint, accessType = VariableAccessType.Readonly, pollIntervalMs = 60000, units = "mA" },
                new VariableDTO() { variableId = IOManager.AC3_VARIABLE_ID, description = "Current input 3", 
                    type = VariableType.FloatingPoint, accessType = VariableAccessType.Readonly, pollIntervalMs = 60000, units = "mA" },
                new VariableDTO() { variableId = IOManager.AC4_VARIABLE_ID, description = "Current input 4", 
                    type = VariableType.FloatingPoint, accessType = VariableAccessType.Readonly, pollIntervalMs = 60000, units = "mA" },
                new VariableDTO() { variableId = IOManager.DO1_VARIABLE_ID, description = "Digital output 1", 
                    type = VariableType.Binary, accessType = VariableAccessType.ReadWrite },
                new VariableDTO() { variableId = IOManager.DO2_VARIABLE_ID, description = "Digital output 2", 
                    type = VariableType.Binary, accessType = VariableAccessType.ReadWrite },
                new VariableDTO() { variableId = IOManager.DO3_VARIABLE_ID, description = "Digital output 3", 
                    type = VariableType.Binary, accessType = VariableAccessType.ReadWrite },
                new VariableDTO() { variableId = IOManager.DO4_VARIABLE_ID, description = "Digital output 4", 
                    type = VariableType.Binary, accessType = VariableAccessType.ReadWrite },
            };
            foreach (var item in result)
                item.source = VariableSource.BuiltIn;
            return result;
        }

        private VariableDTO MakeVariable(DataRow r)
        {
            // Load basic data
            int variableId = (int)r.Field<long>("variable_id");
            var result = new VariableDTO
            {
                variableId = variableId,
                description = r.Field<string>("description"),
                variableCode = r.Field<string>("variable_code"),
                type = (VariableType)r.Field<long>("variable_type"),
                accessType = (VariableAccessType)r.Field<long>("access_type"),
                units = r.Field<string>("units"),
                source = (variableId < 1000) ? VariableSource.BuiltIn : (VariableSource)r.Field<long>("variable_source"),
                pollIntervalMs = (int)r.Field<long>("poll_interval_ms"),
            };

            // Load modbus data if necessary
            if (result.source == VariableSource.Modbus)
            {
                result.modbusRegister = new ModbusRegisterDTO()
                {
                    tcpHost = r.Field<string>("modbus_tcp_host"),
                    slaveId = (int)r.Field<long>("modbus_slave_id"),
                    registerAddress = (int)r.Field<long>("modbus_register_address"),
                    registerType = (ModbusRegisterType)r.Field<long>("modbus_register_type"),
                    dataType = (ModbusDataType)r.Field<long>("modbus_data_type"),
                };
            }

            return result;
        }

        private void LoadVariables(bool forceReload = false)
        {
            lock (_lock)
            {
                if (forceReload && variables != null)
                {
                    variables.Clear();
                    variables = null;
                }
                if (variables == null)
                {
                    variables = Context.Db.GetDataTable("select * from variables")
                        .AsEnumerable()
                        .Select(row => MakeVariable(row))
                        .ToDictionary(item => item.variableId);

                    // Update signature to signal the change
                    variablesSignature++;
                }
            }
        }

        private List<VariableDTO> _GetVariables()
        {
            lock (_lock)
            {
                LoadVariables();
                return variables.Select(item => item.Value).ToList();      // Clone
            }
        }

        private void _CreateVariable(VariableDTO v)
        {
            Context.Db.Execute(
                "insert into variables (variable_id, description, variable_code, variable_type, variable_source, access_type, units, poll_interval_ms) " +
                "values (@variable_id, @description, @variable_code, @variable_type, @variable_source, @access_type, @units, @poll_interval_ms)",
                new()
                {
                    { "@variable_id", v.variableId },
                    { "@description", v.description },
                    { "@variable_code", string.IsNullOrWhiteSpace(v.variableCode) ? DBNull.Value : v.variableCode.Trim() },
                    { "@variable_type", (int)v.type },
                    { "@variable_source", (int)v.source },
                    { "@access_type", (int)v.accessType },
                    { "@units", string.IsNullOrWhiteSpace(v.units) ? DBNull.Value : v.units.Trim() },
                    { "@poll_interval_ms", v.pollIntervalMs },
                });
            _EditModbusData(v.variableId, v.modbusRegister);
        }

        private void _EditVariable(VariableDTO v)
        {
            Context.Db.Execute(
                "update variables set description = @description, variable_code = @variable_code, access_type = @access_type, units = @units, poll_interval_ms = @poll_interval_ms " +
                "where variable_id = @variable_id",
                new()
                {
                    { "@description", v.description },
                    { "@variable_code", string.IsNullOrWhiteSpace(v.variableCode) ? DBNull.Value : v.variableCode.Trim() },
                    { "@access_type", (int)v.accessType },
                    { "@units", string.IsNullOrWhiteSpace(v.units) ? DBNull.Value : v.units.Trim() },
                    { "@poll_interval_ms", v.pollIntervalMs },
                    { "@variable_id", v.variableId },
                });
            _EditModbusData(v.variableId, v.modbusRegister);
        }

        private void _EditModbusData(int variableId, ModbusRegisterDTO md)
        {
            Context.Db.Execute(
                "update variables set modbus_tcp_host = @modbus_tcp_host, modbus_slave_id = @modbus_slave_id, modbus_register_type = @modbus_register_type, " +
                "modbus_register_address = @modbus_register_address, modbus_data_type = @modbus_data_type " +
                "where variable_id = @variable_id",
                new()
                {
                    { "@modbus_tcp_host", string.IsNullOrWhiteSpace(md?.tcpHost) ? DBNull.Value : md.tcpHost.Trim() },
                    { "@modbus_slave_id", md == null ? DBNull.Value : md.slaveId },
                    { "@modbus_register_type", md == null ? DBNull.Value : (int)md.registerType },
                    { "@modbus_register_address", md == null ? DBNull.Value : md.registerAddress },
                    { "@modbus_data_type", md == null ? DBNull.Value : (int)md.dataType },
                    { "@variable_id", variableId },
                });
        }

        #endregion

        #region In-memory variable definitions

        public static List<BasicVariableDefinition> GetInMemoryVariableDefinitions(ref long currentSignature)
        {
            lock (_lock)
            {
                if (currentSignature == variablesSignature || variables == null)
                    return null;

                // Return a cloned list with the basic data of the variables
                currentSignature = variablesSignature;
                return variables.Values.Select(item =>
                {
                    var result = new BasicVariableDefinition()
                    {
                        variableId = item.variableId,
                        source = item.source,
                        type = item.type,
                        accessType = item.accessType,
                        pollIntervalMs = item.pollIntervalMs
                    };
                    if (result.source == VariableSource.Modbus)
                    {
                        result.modbusRegister = new ModbusRegisterDTO()
                        {
                            tcpHost = item.modbusRegister.tcpHost,
                            slaveId = item.modbusRegister.slaveId,
                            registerAddress = item.modbusRegister.registerAddress,
                            registerType = item.modbusRegister.registerType,
                            dataType = item.modbusRegister.dataType,
                        };
                    }
                    return result;
                })
                .ToList();
            }
        }

        #endregion

        #region CRUD

        public List<VariableDTO> EnumerateVariables()
        {
            return _GetVariables();
        }

        public VariableDTO GetVariable(int VariableId)
        {
            lock (_lock)
            {
                LoadVariables();
                if (variables.TryGetValue(VariableId, out VariableDTO Variable))
                    return Variable;
                return null;
            }
        }

        public VariableDTO GetVariableByCode(string variableCode)
        {
            lock (_lock)
            {
                LoadVariables();
                if (string.IsNullOrWhiteSpace(variableCode)) 
                    return null;
                variableCode = variableCode.Trim();
                return variables.Values.FirstOrDefault(item => string.Equals(item.variableCode, variableCode, System.StringComparison.OrdinalIgnoreCase));
            }
        }

        public VariableDTO CreateVariable(VariableDTO v)
        {
            lock (_lock)
            {
                LoadVariables(true);

                // Check for duplicate variable code
                if (!string.IsNullOrWhiteSpace(v.variableCode) && GetVariableByCode(v.variableCode) != null)
                    throw new ControllerException(Context.ErrorCodes.Variables.DUPLICATE_VARIABLE_CODE);

                // Store and reload
                v.variableId = variables.Select(item => item.Value.variableId).Max() + 1;
                if (v.variableId < 1001)
                    v.variableId = 1001;
                _CreateVariable(v);
                LoadVariables(true);

                // Done
                return variables[v.variableId];
            }
        }

        public VariableDTO EditVariable(VariableDTO v)
        {
            lock (_lock)
            {
                LoadVariables(true);

                // Check for duplicate variable code
                if (!string.IsNullOrWhiteSpace(v.variableCode))
                {
                    var v2 = GetVariableByCode(v.variableCode);
                    if (v2 != null && v2.variableId != v.variableId)
                        throw new ControllerException(Context.ErrorCodes.Variables.DUPLICATE_VARIABLE_CODE);
                }

                var newVar = variables[v.variableId];
                newVar.description = v.description;
                newVar.variableCode = v.variableCode;

                // Units cannot be changed for built-in variables
                if (newVar.source == VariableSource.BuiltIn)
                {
                    // Do not change
                }
                else
                {
                    newVar.units = v.units;
                }

                // Access type cannot be changed for built-in variables
                if (newVar.source != VariableSource.BuiltIn)
                {
                    // Do not change
                }
                else
                {
                    newVar.accessType = v.accessType;
                }

                // Poll interval cannot be changed (must be zero) for:
                // - External variables
                // - Built-in binary variables
                if (newVar.source == VariableSource.External || (newVar.source == VariableSource.BuiltIn && newVar.type == VariableType.Binary))
                {
                    newVar.pollIntervalMs = 0;
                }
                else
                {
                    newVar.pollIntervalMs = v.pollIntervalMs;
                }

                // Store and reload
                _EditVariable(v);
                LoadVariables(true);

                // Done
                return variables[v.variableId];
            }
        }

        public void DeleteVariable(int variableId)
        {
            Context.Db.Execute(
                "delete from variables where variable_id = @variable_id",
                new()
                {
                    { "@variable_id", variableId }
                });
            LoadVariables(true);
        }

        #endregion

        #region Variable values

        public List<VariableValueDTO> GetVariableValues()
        {
            var varValues = IOManager.GetVariableValues();
            lock (_lock)
            {
                LoadVariables();
                return variables.Values.Select(v =>
                {
                    if (varValues.TryGetValue(v.variableId, out var value))
                        return value;
                    return new VariableValueDTO() { variableId = v.variableId };
                })
                .OrderBy(item => item.variableId)
                .ToList();
            }
        }

        #endregion

        #region Startup fixes and housekeeping

        public void DoStartupFixes()
        {
            var varsCreated = false;
            foreach (var v in EnumerateBuiltInVariables())
            {
                if (GetVariable(v.variableId) == null)
                {
                    _CreateVariable(v);
                    varsCreated = true;
                    Logger.LogInformation($"Created missing variable {v.variableId}: {v.description}");
                }
            }
            if (varsCreated)
                LoadVariables(true);
        }

        #endregion

    }

}

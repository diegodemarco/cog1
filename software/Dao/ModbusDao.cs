using System.Collections.Generic;
using System.Data;
using cog1.Business;
using cog1.DTO;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;

namespace cog1.Dao
{
    /*

    /// <summary>
    /// Dao class for handling Modbus register configuration
    /// </summary>
    public class ModbusDao : DaoBase
    {
        private static object _lock = new object();
        private static long registersSignature = 0;
        private static Dictionary<int, ModbusRegisterDTO> registers = null;
        private static Random random = new();

        public ModbusDao(Cog1Context context, ILogger logger) : base(context, logger)
        {
        }

        #region Private methods

        private ModbusRegisterDTO MakeRegister(DataRow r)
        {
            int registerId = (int)r.Field<long>("modbus_register_id");
            return new ModbusRegisterDTO
            {
                modbusRegisterId = registerId,
                description = r.Field<string>("description"),
                tcpHost = r.Field<string>("tcp_host"),
                slaveId = (int)r.Field<long>("slave_id"),
                registerType = (ModbusRegisterType)r.Field<long>("register_type"),
                registerAddress = (int)r.Field<long>("register_address"),
                dataType = (ModbusDataType)r.Field<long>("data_type"),
            };
        }

        private void LoadRegisters(bool forceReload = false)
        {
            lock (_lock)
            {
                if (forceReload && registers != null)
                {
                    registers.Clear();
                    registers = null;
                }
                if (registers == null)
                {
                    registers = Context.Db.GetDataTable("select * from modbus_registers")
                        .AsEnumerable()
                        .Select(row => MakeRegister(row))
                        .ToDictionary(item => item.modbusRegisterId);

                    // Update signature to signal the change
                    registersSignature = random.NextInt64();
                }
            }
        }

        private List<ModbusRegisterDTO> _GetRegisters()
        {
            lock (_lock)
            {
                LoadRegisters();
                return registers.Select(item => item.Value).ToList();      // Clone
            }
        }

        private void _CreateRegister(ModbusRegisterDTO r)
        {
            Context.Db.Execute(
                "insert into modbus_registers (modbus_register_id, description, tcp_host, slave_id, register_type, register_address, data_type) " +
                "values (@modbus_register_id, @description, @tcp_host, @slave_id, @register_type, @register_address, @data_type)",
                new()
                {
                    { "@modbus_register_id", r.modbusRegisterId },
                    { "@description", r.description },
                    { "@tcp_host", string.IsNullOrWhiteSpace(r.tcpHost) ? DBNull.Value : r.tcpHost.Trim() },
                    { "@slave_id", r.slaveId },
                    { "@register_type", (int)r.registerType },
                    { "@register_address", r.registerAddress },
                    { "@data_type", (int)r.dataType },
                });
        }

        private void _EditRegister(ModbusRegisterDTO r)
        {
            Context.Db.Execute(
                "update modbus_registers set description = @description, tcp_host = @tcp_host, slave_id = @slave_id, " +
                "register_type = @register_type, register_address = @register_address, data_type = @data_type " +
                "where modbus_register_id = @modbus_register_id",
                new()
                {
                    { "@modbus_register_id", r.modbusRegisterId},
                    { "@description", r.description },
                    { "@tcp_host", string.IsNullOrWhiteSpace(r.tcpHost) ? DBNull.Value : r.tcpHost.Trim() },
                    { "@slave_id", r.slaveId },
                    { "@register_type", (int)r.registerType },
                    { "@register_address", r.registerAddress },
                    { "@data_type", (int)r.dataType },
                });
        }

        #endregion

        #region In-memory register definitions

        public static List<ModbusRegisterDTO> GetInMemoryRegisterDefinitions(ref long currentSignature)
        {
            lock (_lock)
            {
                if (currentSignature == registersSignature || registers == null)
                    return null;
                // Return a cloned list with the basic data of the registers
                currentSignature = registersSignature;
                return registers.Values.Select(item => new ModbusRegisterDTO()
                {
                    modbusRegisterId = item.modbusRegisterId,
                    description = item.description,
                    tcpHost = item.tcpHost,
                    slaveId = item.slaveId,
                    registerType = item.registerType,
                    registerAddress = item.registerAddress,
                    dataType = item.dataType,
                }).ToList();
            }
        }

        #endregion

        #region CRUD

        public List<ModbusRegisterDTO> EnumerateRegisters()
        {
            return _GetRegisters();
        }

        public ModbusRegisterDTO GetRegister(int registerId)
        {
            lock (_lock)
            {
                LoadRegisters();
                if (registers.TryGetValue(registerId, out ModbusRegisterDTO register))
                    return register;
                return null;
            }
        }

        public ModbusRegisterDTO CreateRegister(ModbusRegisterDTO r)
        {
            lock (_lock)
            {
                LoadRegisters(true);

                // Store
                r.modbusRegisterId = 0;
                if (registers.Any())
                    r.modbusRegisterId = registers.Select(item => item.Value.modbusRegisterId).Max() + 1;
                if (r.modbusRegisterId < 1001)
                    r.modbusRegisterId = 1001;
                _CreateRegister(r);
                LoadRegisters(true);
                return registers[r.modbusRegisterId];
            }
        }

        public ModbusRegisterDTO EditRegister(ModbusRegisterDTO r)
        {
            lock (_lock)
            {
                LoadRegisters(true);

                var newVar = registers[r.modbusRegisterId];
                newVar.description = r.description;
                newVar.tcpHost = r.tcpHost;
                newVar.slaveId = r.slaveId;
                newVar.registerType = r.registerType;
                newVar.registerAddress = r.registerAddress;
                newVar.dataType = r.dataType;

                _EditRegister(r);
                LoadRegisters(true);
                return registers[r.modbusRegisterId];
            }
        }

        public void DeleteRegister(int registerId)
        {
            Context.Db.Execute(
                "delete from modbus_registers where modbus_register_id = @modbus_register_id",
                new()
                {
                    { "@modbus_register_id", registerId }
                });
            LoadRegisters(true);
        }

        #endregion

    }

    */

}

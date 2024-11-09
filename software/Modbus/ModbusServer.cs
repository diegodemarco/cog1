using System.Linq;
using System;

namespace cog1.Modbus
{
    public abstract class ModbusServer
    {

        #region Constants

        protected const byte FUNCTION_READ_COIL = 1;
        protected const byte FUNCTION_READ_DISCRETE_INPUT = 2;
        protected const byte FUNCTION_READ_HOLDING_REGISTER = 3;
        protected const byte FUNCTION_READ_INPUT_REGISTER = 4;
        protected const byte FUNCTION_WRITE_SINGLE_COIL = 5;
        protected const byte FUNCTION_WRITE_SINGLE_HOLDING_REGISTER = 6;
        protected const byte FUNCTION_WRITE_MULTIPLE_HOLDING_REGISTERS = 16;

        protected const byte ERROR_ILLEGAL_FUNCTION = 1;
        protected const byte ERROR_ILLEGAL_DATA_ADDRESS = 2;
        protected const byte ERROR_ILLEGAL_DATA_VALUE = 3;
        protected const byte ERROR_SLAVE_DEVICE_FAILURE = 4;
        protected const byte ERROR_ACKNOWLEDGE = 5;
        protected const byte ERROR_SLAVE_DEVICE_BUSY = 6;
        protected const byte ERROR_NEGATIVE_ACKNOWLEDGE = 7;
        protected const byte ERROR_MEMORY_PARITY_ERROR = 8;
        protected const byte ERROR_GATEWAY_PATH_UNAVAILABLE = 10;
        protected const byte ERROR_GATEWAY_TARGET_DEFICE_FAILED_TO_RESPOND = 11;

        #endregion

        #region Abstract communication methods

        protected abstract bool ExchangeData(object conn, byte[] requestData, out byte[] responseData);

        #endregion

        #region Error handling

        private ModbusErrorInfo errorInfo = new();

        protected void SetErrorInfo(string errorMessage)
        {
            errorInfo.ModbusErrorCode = 0;
            errorInfo.ErrorMessage = errorMessage;
        }

        protected void SetErrorInfo(byte modbusErrorCode)
        {
            errorInfo.ModbusErrorCode = modbusErrorCode;
            errorInfo.ErrorMessage = GetModbusErrorDescription(modbusErrorCode);
        }

        protected void SetErrorInfo(byte modbusErrorCode, string errorMessage)
        {
            errorInfo.ModbusErrorCode = modbusErrorCode;
            errorInfo.ErrorMessage = errorMessage;
        }

        protected void SetErrorInfoBadChecksum()
        {
            SetErrorInfo("Bad checksum");
        }

        protected void SetErrorInfoTimeout()
        {
            SetErrorInfo("Timeout reading from slave");
        }

        private string GetModbusErrorDescription(byte modbusErrorCode)
        {
            switch (modbusErrorCode)
            {
                case ERROR_ILLEGAL_FUNCTION:
                    return "Illegal function";
                case ERROR_ILLEGAL_DATA_ADDRESS:
                    return "Illegal data address";
                case ERROR_ILLEGAL_DATA_VALUE:
                    return "Illegal data value";
                case ERROR_SLAVE_DEVICE_FAILURE:
                    return "Slave device failure";
                case ERROR_ACKNOWLEDGE:
                    return "Acknowledge";
                case ERROR_SLAVE_DEVICE_BUSY:
                    return "Slave device busy";
                case ERROR_NEGATIVE_ACKNOWLEDGE:
                    return "Negative acknowledge";
                case ERROR_MEMORY_PARITY_ERROR:
                    return "Memory parity error";
                case ERROR_GATEWAY_PATH_UNAVAILABLE:
                    return "Gateway path unavailable";
                case ERROR_GATEWAY_TARGET_DEFICE_FAILED_TO_RESPOND:
                    return "Gateway target device failed to respond";
                default:
                    return $"Unknown Modbus error code {modbusErrorCode}";
            }
        }

        public string ErrorMessage => errorInfo.ErrorMessage;

        #endregion

        #region Generic register read/write

        protected bool ReadRegisters(object conn, byte slaveAddress, byte function, UInt16 registerAddress, UInt16 registerCount, byte expectedBytes, out byte[] data)
        {
            var registerAddressData = FromUInt16((UInt16)(registerAddress - 1));        // Register IDs are zero-based
            var registerCountData = FromUInt16(registerCount);

            // Prepare output payload
            byte[] reqData = new byte[1 + 1 + 2 + 2];       // slave + function + registerAddress + registerCount
            byte i = 0;
            reqData[i++] = slaveAddress;
            reqData[i++] = function;
            reqData[i++] = registerAddressData[0];
            reqData[i++] = registerAddressData[1];
            reqData[i++] = registerCountData[0];
            reqData[i++] = registerCountData[1];

            // Exchange data
            byte[] rspData;
            if (!ExchangeData(conn, reqData, out rspData))
            {
                // Error info must have been set by ExchangeData()
                data = Array.Empty<byte>();
                return false;
            }

            // Ensure response is from the expected slave
            if (rspData[0] != slaveAddress)
            {
                SetErrorInfo($"Unexpected response from slave {rspData[0]} instead of {slaveAddress}");
                data = Array.Empty<byte>();
                return false;
            }

            // Ensure response is for the expected function code
            if ((rspData[1] & 0b01111111) != function)
            {
                SetErrorInfo($"Unexpected response function {rspData[1]} instead of {function}");
                data = Array.Empty<byte>();
                return false;
            }

            // Check for slave-reported errors
            if ((rspData[1] & 128) != 0)
            {
                SetErrorInfo(rspData[2]);
                data = Array.Empty<byte>();
                return false;
            }

            // Check that received data bytes match what is expected
            if (rspData[2] != expectedBytes)
            {
                SetErrorInfo($"Expected {expectedBytes} data bytes but {rspData[2]} data bytes were received");
                data = Array.Empty<byte>();
                return false;
            }

            // Read data bytes
            data = new byte[rspData[2]];
            for (i = 0; i < data.Length; i++)
                data[i] = rspData[i + 3];

            // Done
            return true;
        }

        protected bool WriteSingleRegister(object conn, byte slaveAddress, byte function, UInt16 registerAddress, UInt16 data)
        {
            var registerAddressData = FromUInt16((UInt16)(registerAddress - 1));        // Register IDs are zero-based
            var registerData = FromUInt16(data);

            // Prepare output payload
            byte[] reqData = new byte[1 + 1 + 2 + 2];       // slave + function + registerAddress + registerCount
            byte i = 0;
            reqData[i++] = slaveAddress;
            reqData[i++] = function;
            reqData[i++] = registerAddressData[0];
            reqData[i++] = registerAddressData[1];
            reqData[i++] = registerData[0];
            reqData[i++] = registerData[1];

            // Exchange data
            byte[] rspData;
            if (!ExchangeData(conn, reqData, out rspData))
            {
                // Error info must have been set by ExchangeData()
                return false;
            }

            // Ensure response is from the expected slave
            if (rspData[0] != slaveAddress)
            {
                SetErrorInfo($"Unexpected response from slave {rspData[0]} instead of {slaveAddress}");
                return false;
            }

            // Ensure response is for the expected function code
            if ((rspData[1] & 0b01111111) != function)
            {
                SetErrorInfo($"Unexpected response function {rspData[1]} instead of {function}");
                return false;
            }

            // Check for slave-reported errors
            if ((rspData[1] & 128) != 0)
            {
                SetErrorInfo(rspData[2]);
                return false;
            }

            // Done
            return true;
        }

        protected bool WriteMultipleRegisters(object conn, byte slaveAddress, byte function, UInt16 registerAddress, UInt16 registerCount, byte[] data)
        {
            return false;
        }

        #endregion

        #region Coils

        protected bool ReadCoil(object conn, byte slaveAddress, UInt16 registerAddress, out bool value)
        {
            if (ReadRegisters(conn, slaveAddress, FUNCTION_READ_COIL, registerAddress, 1, 1, out var data))
            {
                value = (data[0] != 0);
                return true;
            }
            value = false;
            return false;
        }

        protected bool WriteCoil(object conn, byte slaveAddress, UInt16 registerAddress, bool value)
        {
            return WriteSingleRegister(conn, slaveAddress, FUNCTION_WRITE_SINGLE_COIL, registerAddress, value ? (UInt16)0xff00 : (UInt16)0x0000);
        }

        #endregion

        #region Discrete inputs

        protected bool ReadDiscreteInput(object conn, byte slaveAddress, UInt16 registerAddress, out bool value)
        {
            if (ReadRegisters(conn, slaveAddress, FUNCTION_READ_DISCRETE_INPUT, registerAddress, 1, 1, out var data))
            {
                value = (data[0] != 0);
                return true;
            }
            value = false;
            return false;
        }

        #endregion

        #region Holding registers

        protected bool ReadHoldingRegisterUInt16(object conn, byte slaveAddress, UInt16 registerAddress, out UInt16 value)
        {
            if (ReadRegisters(conn, slaveAddress, FUNCTION_READ_HOLDING_REGISTER, registerAddress, 1, 2, out var data))
            {
                value = ToUInt16(data);
                return true;
            }
            value = 0;
            return false;
        }

        protected bool WriteHoldingRegisterUInt16(object conn, byte slaveAddress, UInt16 registerAddress, UInt16 value)
        {
            return WriteSingleRegister(conn, slaveAddress, FUNCTION_WRITE_SINGLE_HOLDING_REGISTER, registerAddress, value);
        }

        protected bool ReadHoldingRegisterInt16(object conn, byte slaveAddress, UInt16 registerAddress, out Int16 value)
        {
            if (ReadRegisters(conn, slaveAddress, FUNCTION_READ_HOLDING_REGISTER, registerAddress, 1, 2, out var data))
            {
                value = ToInt16(data);
                return true;
            }
            value = 0;
            return false;
        }

        protected bool WriteHoldingRegisterInt16(object conn, byte slaveAddress, UInt16 registerAddress, Int16 value)
        {
            return WriteSingleRegister(conn, slaveAddress, FUNCTION_WRITE_SINGLE_HOLDING_REGISTER, registerAddress, (UInt16)value);
        }

        protected bool ReadHoldingRegisterUInt32(object conn, byte slaveAddress, UInt16 registerAddress, out UInt32 value)
        {
            if (ReadRegisters(conn, slaveAddress, FUNCTION_READ_HOLDING_REGISTER, registerAddress, 2, 4, out var data))
            {
                value = ToUInt32(data);
                return true;
            }
            value = 0;
            return false;
        }

        protected bool WriteHoldingRegisterUInt32(object conn, byte slaveAddress, UInt16 registerAddress, UInt32 value)
        {
            return WriteMultipleRegisters(conn, slaveAddress, FUNCTION_WRITE_MULTIPLE_HOLDING_REGISTERS, registerAddress, 2, FromUInt32(value));
        }

        protected bool ReadHoldingRegisterInt32(object conn, byte slaveAddress, UInt16 registerAddress, out Int32 value)
        {
            if (ReadRegisters(conn, slaveAddress, FUNCTION_READ_HOLDING_REGISTER, registerAddress, 2, 4, out var data))
            {
                value = ToInt32(data);
                return true;
            }
            value = 0;
            return false;
        }

        protected bool WriteHoldingRegisterInt32(object conn, byte slaveAddress, UInt16 registerAddress, Int32 value)
        {
            return WriteMultipleRegisters(conn, slaveAddress, FUNCTION_WRITE_MULTIPLE_HOLDING_REGISTERS, registerAddress, 2, FromInt32(value));
        }

        protected bool ReadHoldingRegisterFloat32(object conn, byte slaveAddress, UInt16 registerAddress, out Single value)
        {
            if (ReadRegisters(conn, slaveAddress, FUNCTION_READ_HOLDING_REGISTER, registerAddress, 2, 4, out var data))
            {
                value = ToFloat32(data);
                return true;
            }
            value = 0;
            return false;
        }

        protected bool WriteHoldingRegisterFloat(object conn, byte slaveAddress, UInt16 registerAddress, Single value)
        {
            return WriteMultipleRegisters(conn, slaveAddress, FUNCTION_WRITE_MULTIPLE_HOLDING_REGISTERS, registerAddress, 2, FromFloat32(value));
        }

        #endregion

        #region Input registers

        protected bool ReadInputRegisterUInt16(object conn, byte slaveAddress, UInt16 registerAddress, out UInt16 value)
        {
            if (ReadRegisters(conn, slaveAddress, FUNCTION_READ_INPUT_REGISTER, registerAddress, 1, 2, out var data))
            {
                value = ToUInt16(data);
                return true;
            }
            value = 0;
            return false;
        }

        protected bool ReadInputRegisterInt16(object conn, byte slaveAddress, UInt16 registerAddress, out Int16 value)
        {
            if (ReadRegisters(conn, slaveAddress, FUNCTION_READ_INPUT_REGISTER, registerAddress, 1, 2, out var data))
            {
                value = ToInt16(data);
                return true;
            }
            value = 0;
            return false;
        }

        protected bool ReadInputRegisterUInt32(object conn, byte slaveAddress, UInt16 registerAddress, out UInt32 value)
        {
            if (ReadRegisters(conn, slaveAddress, FUNCTION_READ_INPUT_REGISTER, registerAddress, 2, 4, out var data))
            {
                value = ToUInt32(data);
                return true;
            }
            value = 0;
            return false;
        }

        protected bool ReadInputRegisterInt32(object conn, byte slaveAddress, UInt16 registerAddress, out Int32 value)
        {
            if (ReadRegisters(conn, slaveAddress, FUNCTION_READ_INPUT_REGISTER, registerAddress, 2, 4, out var data))
            {
                value = ToInt32(data);
                return true;
            }
            value = 0;
            return false;
        }

        protected bool ReadInputRegisterFloat32(object conn, byte slaveAddress, UInt16 registerAddress, out Single value)
        {
            if (ReadRegisters(conn, slaveAddress, FUNCTION_READ_INPUT_REGISTER, registerAddress, 2, 4, out var data))
            {
                value = ToFloat32(data);
                return true;
            }
            value = 0;
            return false;
        }

        #endregion

        #region Data conversion

        protected static UInt16 ToUInt16(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToUInt16(data.Reverse().ToArray());
            return BitConverter.ToUInt16(data);
        }

        protected static Int16 ToInt16(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToInt16(data.Reverse().ToArray());
            return BitConverter.ToInt16(data);
        }

        protected static UInt32 ToUInt32(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToUInt32(data.Reverse().ToArray());
            return BitConverter.ToUInt32(data);
        }

        protected static Int32 ToInt32(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToInt32(data.Reverse().ToArray());
            return BitConverter.ToInt32(data);
        }

        protected static Single ToFloat32(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToSingle(data.Reverse().ToArray());
            return BitConverter.ToSingle(data);
        }

        protected static byte[] FromUInt16(UInt16 value)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value).Reverse().ToArray();
            return BitConverter.GetBytes(value);
        }

        protected static byte[] FromInt16(Int16 value)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value).Reverse().ToArray();
            return BitConverter.GetBytes(value);
        }

        protected static byte[] FromUInt32(UInt32 value)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value).Reverse().ToArray();
            return BitConverter.GetBytes(value);
        }

        protected static byte[] FromInt32(Int32 value)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value).Reverse().ToArray();
            return BitConverter.GetBytes(value);
        }

        protected static byte[] FromFloat32(Single value)
        {
            if (BitConverter.IsLittleEndian)
                return BitConverter.GetBytes(value).Reverse().ToArray();
            return BitConverter.GetBytes(value);
        }

        #endregion

    }
}

using System.Linq;
using System;
using System.Net.Sockets;

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

        protected abstract bool ExchangeData(string tcpHost, byte[] requestData, out byte[] responseData);
        protected abstract bool ReadChars(Socket socket, byte[] buffer, ref int offset, int byteCount);
        protected abstract bool StripChecksum(byte[] arr, int len, out byte[] data);

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

        protected bool ReadResponseMessage(Socket socket, out byte[] responseData, bool useChecksum)
        {
            // Read the response
            int index = 0;
            var buffer = new byte[256];
            responseData = Array.Empty<byte>();

            // Read device address and function code
            if (!ReadChars(socket, buffer, ref index, 2))
            {
                Console.WriteLine($"Timeout reading device address and function code => [{Utils.BytesToHex(buffer, index)}]");
                return false;
            }
            var functionCode = buffer[1];

            // Check for error
            if (functionCode >= 0x80)
            {
                // Read three bytes (error code and checksum) to complete
                if (!ReadChars(socket, buffer, ref index, 1 + (useChecksum ? 2 : 0)))
                {
                    Console.WriteLine($"Timeout reading error details => [{Utils.BytesToHex(buffer, index)}]");
                    return false;
                }
                Console.WriteLine($"Error => [{Utils.BytesToHex(buffer, index)}]");
                return StripChecksum(buffer, index, out responseData);
            }

            // Read content
            switch (functionCode)
            {
                case FUNCTION_READ_COIL:
                case FUNCTION_READ_DISCRETE_INPUT:
                case FUNCTION_READ_HOLDING_REGISTER:
                case FUNCTION_READ_INPUT_REGISTER:
                    // Read number of bytes following
                    if (!ReadChars(socket, buffer, ref index, 1))
                    {
                        Console.WriteLine($"Timeout reading number of bytes => [{Utils.BytesToHex(buffer, index)}]");
                        return false;
                    }
                    var byteCount = buffer[index - 1];
                    // Read the specified number of chars, plus the checksum, to complete
                    if (!ReadChars(socket, buffer, ref index, byteCount + (useChecksum ? 2 : 0)))    // Data bytes + checksum
                    {
                        Console.WriteLine($"Timeout reading specified number of bytes => [{Utils.BytesToHex(buffer, index)}]");
                        return false;
                    }
                    return StripChecksum(buffer, index, out responseData);

                case FUNCTION_WRITE_SINGLE_COIL:
                    // Read the rest of the frame
                    if (!ReadChars(socket, buffer, ref index, 2 + 2 + (useChecksum ? 2 : 0)))        // Register address + register value + checksum
                    {
                        Console.WriteLine($"Timeout reading write single coil bytes => [{Utils.BytesToHex(buffer, index)}]");
                        return false;
                    }
                    return StripChecksum(buffer, index, out responseData);

                case FUNCTION_WRITE_SINGLE_HOLDING_REGISTER:
                    // Read the rest of the frame
                    if (!ReadChars(socket, buffer, ref index, 2 + 2 + (useChecksum ? 2 : 0)))        // Register address + register value + checksum
                    {
                        Console.WriteLine($"Timeout reading write single holding register bytes => [{Utils.BytesToHex(buffer, index)}]");
                        return false;
                    }
                    return StripChecksum(buffer, index, out responseData);

                case FUNCTION_WRITE_MULTIPLE_HOLDING_REGISTERS:
                    // Read the rest of the frame
                    if (!ReadChars(socket, buffer, ref index, 2 + 2 + (useChecksum ? 2 : 0)))        // Register address + register count + checksum
                    {
                        Console.WriteLine($"Timeout reading write multiple holding register bytes => [{Utils.BytesToHex(buffer, index)}]");
                        return false;
                    }
                    return StripChecksum(buffer, index, out responseData);

                default:
                    SetErrorInfo($"Unsupported response function code {functionCode}");
                    return false;
            }
        }

        private bool CheckBasicResponseAspects(byte[] rspData, UInt16 slaveAddress, byte function)
        {
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

            // All good
            return true;
        }

        protected bool ReadRegisters(string tcpHost, byte slaveAddress, byte function, UInt16 registerAddress, UInt16 registerCount, byte expectedBytes, out byte[] data)
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
            if (!ExchangeData(tcpHost, reqData, out rspData))
            {
                // Error info must have been set by ExchangeData()
                data = Array.Empty<byte>();
                return false;
            }

            // Do basic checks on the received response
            if (!CheckBasicResponseAspects(rspData, slaveAddress, function))
            {
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

        protected bool WriteSingleRegister(string tcpHost, byte slaveAddress, byte function, UInt16 registerAddress, UInt16 data)
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
            if (!ExchangeData(tcpHost, reqData, out rspData))
            {
                // Error info must have been set by ExchangeData()
                return false;
            }

            // Do basic checks on the received response
            if (!CheckBasicResponseAspects(rspData, slaveAddress, function))
                return false;

            // Done
            return true;
        }

        protected bool WriteMultipleRegisters(string tcpHost, byte slaveAddress, byte function, UInt16 registerAddress, UInt16 registerCount, byte[] data)
        {
            //SetErrorInfo("Modbus WriteMultipleRegisters: not implemented");
            var registerAddressData = FromUInt16((UInt16)(registerAddress - 1));        // Register IDs are zero-based
            var registerCountData = FromUInt16(registerCount);

            // Prepare output payload
            byte[] reqData = new byte[1 + 1 + 2 + 2 + 1 + data.Length];       // slave + function + registerAddress + registerCount + dataByteCount + data
            byte i = 0;
            reqData[i++] = slaveAddress;
            reqData[i++] = function;
            reqData[i++] = registerAddressData[0];
            reqData[i++] = registerAddressData[1];
            reqData[i++] = registerCountData[0];
            reqData[i++] = registerCountData[1];
            reqData[i++] = (byte)data.Length;
            foreach(var b in data)
                reqData[i++] = b;

            // Exchange data
            byte[] rspData;
            if (!ExchangeData(tcpHost, reqData, out rspData))
            {
                // Error info must have been set by ExchangeData()
                return false;
            }

            // Do basic checks on the received response
            if (!CheckBasicResponseAspects(rspData, slaveAddress, function))
                return false;

            // Done
            return true;
        }

        #endregion

        #region Coils

        public bool ReadCoil(string tcpHost, byte slaveAddress, UInt16 registerAddress, out bool value)
        {
            if (ReadRegisters(tcpHost, slaveAddress, FUNCTION_READ_COIL, registerAddress, 1, 1, out var data))
            {
                value = (data[0] != 0);
                return true;
            }
            value = false;
            return false;
        }

        public bool WriteCoil(string tcpHost, byte slaveAddress, UInt16 registerAddress, bool value)
        {
            return WriteSingleRegister(tcpHost, slaveAddress, FUNCTION_WRITE_SINGLE_COIL, registerAddress, value ? (UInt16)0xff00 : (UInt16)0x0000);
        }

        #endregion

        #region Discrete inputs

        public bool ReadDiscreteInput(string tcpHost, byte slaveAddress, UInt16 registerAddress, out bool value)
        {
            if (ReadRegisters(tcpHost, slaveAddress, FUNCTION_READ_DISCRETE_INPUT, registerAddress, 1, 1, out var data))
            {
                value = (data[0] != 0);
                return true;
            }
            value = false;
            return false;
        }

        #endregion

        #region Holding registers

        public bool ReadHoldingRegisterUInt16(string tcpHost, byte slaveAddress, UInt16 registerAddress, out UInt16 value)
        {
            if (ReadRegisters(tcpHost, slaveAddress, FUNCTION_READ_HOLDING_REGISTER, registerAddress, 1, 2, out var data))
            {
                value = ToUInt16(data);
                return true;
            }
            value = 0;
            return false;
        }

        public bool WriteHoldingRegisterUInt16(string tcpHost, byte slaveAddress, UInt16 registerAddress, UInt16 value)
        {
            return WriteSingleRegister(tcpHost, slaveAddress, FUNCTION_WRITE_SINGLE_HOLDING_REGISTER, registerAddress, value);
        }

        public bool ReadHoldingRegisterInt16(string tcpHost, byte slaveAddress, UInt16 registerAddress, out Int16 value)
        {
            if (ReadRegisters(tcpHost, slaveAddress, FUNCTION_READ_HOLDING_REGISTER, registerAddress, 1, 2, out var data))
            {
                value = ToInt16(data);
                return true;
            }
            value = 0;
            return false;
        }

        public bool WriteHoldingRegisterInt16(string tcpHost, byte slaveAddress, UInt16 registerAddress, Int16 value)
        {
            return WriteSingleRegister(tcpHost, slaveAddress, FUNCTION_WRITE_SINGLE_HOLDING_REGISTER, registerAddress, (UInt16)value);
        }

        public bool ReadHoldingRegisterUInt32(string tcpHost, byte slaveAddress, UInt16 registerAddress, out UInt32 value)
        {
            if (ReadRegisters(tcpHost, slaveAddress, FUNCTION_READ_HOLDING_REGISTER, registerAddress, 2, 4, out var data))
            {
                value = ToUInt32(data);
                return true;
            }
            value = 0;
            return false;
        }

        public bool WriteHoldingRegisterUInt32(string tcpHost, byte slaveAddress, UInt16 registerAddress, UInt32 value)
        {
            return WriteMultipleRegisters(tcpHost, slaveAddress, FUNCTION_WRITE_MULTIPLE_HOLDING_REGISTERS, registerAddress, 2, FromUInt32(value));
        }

        public bool ReadHoldingRegisterInt32(string tcpHost, byte slaveAddress, UInt16 registerAddress, out Int32 value)
        {
            if (ReadRegisters(tcpHost, slaveAddress, FUNCTION_READ_HOLDING_REGISTER, registerAddress, 2, 4, out var data))
            {
                value = ToInt32(data);
                return true;
            }
            value = 0;
            return false;
        }

        public bool WriteHoldingRegisterInt32(string tcpHost, byte slaveAddress, UInt16 registerAddress, Int32 value)
        {
            return WriteMultipleRegisters(tcpHost, slaveAddress, FUNCTION_WRITE_MULTIPLE_HOLDING_REGISTERS, registerAddress, 2, FromInt32(value));
        }

        public bool ReadHoldingRegisterFloat32(string tcpHost, byte slaveAddress, UInt16 registerAddress, out Single value)
        {
            if (ReadRegisters(tcpHost, slaveAddress, FUNCTION_READ_HOLDING_REGISTER, registerAddress, 2, 4, out var data))
            {
                value = ToFloat32(data);
                return true;
            }
            value = 0;
            return false;
        }

        public bool WriteHoldingRegisterFloat32(string tcpHost, byte slaveAddress, UInt16 registerAddress, Single value)
        {
            return WriteMultipleRegisters(tcpHost, slaveAddress, FUNCTION_WRITE_MULTIPLE_HOLDING_REGISTERS, registerAddress, 2, FromFloat32(value));
        }

        #endregion

        #region Input registers

        public bool ReadInputRegisterUInt16(string tcpHost, byte slaveAddress, UInt16 registerAddress, out UInt16 value)
        {
            if (ReadRegisters(tcpHost, slaveAddress, FUNCTION_READ_INPUT_REGISTER, registerAddress, 1, 2, out var data))
            {
                value = ToUInt16(data);
                return true;
            }
            value = 0;
            return false;
        }

        public bool ReadInputRegisterInt16(string tcpHost, byte slaveAddress, UInt16 registerAddress, out Int16 value)
        {
            if (ReadRegisters(tcpHost, slaveAddress, FUNCTION_READ_INPUT_REGISTER, registerAddress, 1, 2, out var data))
            {
                value = ToInt16(data);
                return true;
            }
            value = 0;
            return false;
        }

        public bool ReadInputRegisterUInt32(string tcpHost, byte slaveAddress, UInt16 registerAddress, out UInt32 value)
        {
            if (ReadRegisters(tcpHost, slaveAddress, FUNCTION_READ_INPUT_REGISTER, registerAddress, 2, 4, out var data))
            {
                value = ToUInt32(data);
                return true;
            }
            value = 0;
            return false;
        }

        public bool ReadInputRegisterInt32(string tcpHost, byte slaveAddress, UInt16 registerAddress, out Int32 value)
        {
            if (ReadRegisters(tcpHost, slaveAddress, FUNCTION_READ_INPUT_REGISTER, registerAddress, 2, 4, out var data))
            {
                value = ToInt32(data);
                return true;
            }
            value = 0;
            return false;
        }

        public bool ReadInputRegisterFloat32(string tcpHost, byte slaveAddress, UInt16 registerAddress, out Single value)
        {
            if (ReadRegisters(tcpHost, slaveAddress, FUNCTION_READ_INPUT_REGISTER, registerAddress, 2, 4, out var data))
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

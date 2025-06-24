using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace cog1.Modbus
{
    public class ModbusTcpServer() : ModbusServer, IDisposable
    {
        //private const int MIN_INTER_FRAME_DELAY = 20;
        //private int interFrameDelay = MIN_INTER_FRAME_DELAY;
        //private SerialPort serialPort;
        private List<TcpSlave> slaveConnections = new List<TcpSlave>();
        private int transactionId = 0;

        #region Data exchange

        private UInt16  GetTransactionId()
        {
            transactionId++;
            if (transactionId > UInt16.MaxValue)
                transactionId = 0;
            return (UInt16)transactionId;
        }

        protected override bool ReadChars(Socket socket, byte[] buffer, ref int offset, int byteCount)
        {
            if (byteCount <= 0)
                return true;

            var timeout = stopwatch.ElapsedMilliseconds + 6000;        // 6 second timeout
            try
            {
                //Console.WriteLine($"TCP: Waiting for {byteCount} bytes");
                while (timeout > stopwatch.ElapsedMilliseconds)
                {
                    var len = socket.Available;
                    if (len > 0)
                    {
                        if (len > byteCount)
                            len = byteCount;
                        //Console.WriteLine($"TCP: Data: {len} bytes");
                        var count = socket.Receive(buffer, offset, len, SocketFlags.None);
                        offset += count;
                        byteCount -= count;
                        if (byteCount == 0)
                        {
                            //Console.WriteLine("TCP: Success");
                            return true;
                        }
                    }
                    else
                    {
                        Thread.Sleep(5);
                    }
                }
                return false;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"TCP: ReadChars exception: {ex}");
                return false;
            }
        }

        protected override bool ExchangeData(string tcpHost, byte[] requestData, out byte[] responseData)
        {
            // Destroy expired connections
            foreach (var cn in slaveConnections.Where(item => item.Expired).ToArray())
            {
                cn.Dispose();
                slaveConnections.Remove(cn);
            }

            // Find slave connection
            var conn = slaveConnections.FirstOrDefault(item => item.IsHost(tcpHost));
            if (conn == null)
            {
                conn = new TcpSlave(tcpHost);
                slaveConnections.Add(conn);
            }

            try
            {
                var socket = conn.GetSocket(out var connErrorMessage);
                if (socket == null)
                {
                    responseData = Array.Empty<byte>();
                    SetErrorInfo(connErrorMessage);
                    return false;
                }

                // Add tcp header
                var txid = GetTransactionId();
                var len = requestData.Length;
                var newArray = new byte[len + 6];
                Array.Copy(requestData, 0, newArray, 6, len);

                newArray[0] = (byte)(txid >> 8);        // Transaction id (high)
                newArray[1] = (byte)(txid & 0xff);      // Transaction id (low)
                newArray[2] = 0;                        // Protocol identifier (high)
                newArray[3] = 0;                        // Protocol identifier (low)
                newArray[4] = (byte)(len >> 8);         // Data length (high)
                newArray[5] = (byte)(len & 0xff);       // Data length (low)
                //Console.WriteLine($"Frame to be sent with header is [{Utils.BytesToHex(newArray, " ")}]");

                // Send the data frame
                socket.Send(newArray);

                // Read the header (6 bytes)
                int index = 0;
                var buffer = new byte[6];
                if (!ReadChars(socket, buffer, ref index, 6))
                {
                    responseData = Array.Empty<byte>();
                    Console.WriteLine($"Timeout reading 6 byte tcp response header => [{Utils.BytesToHex(buffer, index)}]");
                    return false;
                }

                // Verify the header
                if (buffer[0] != newArray[0] || buffer[1] != newArray[1] || buffer[2] != newArray[2] || buffer[3] != newArray[3])
                {
                    responseData = Array.Empty<byte>();
                    Console.WriteLine($"header mismatch: {Utils.BytesToHex(buffer, " ")} / {Utils.BytesToHex(newArray, " ")}");
                    SetErrorInfo("Reponse header does not match request header");
                    return false;
                }
                //else
                //{
                //    Console.WriteLine("Header check Ok");
                //}

                // Read the response
                var result = ReadResponseMessage(socket, out responseData, false);

                // If everything went well, the tcp slave can continue to be used
                if (result)
                    conn.KickExpiration();

                return result;
            }
            catch (Exception ex)
            {
                responseData = [];
                SetErrorInfo(ex.ToString());
                return false;
            }
        }

        protected override bool StripChecksum(byte[] arr, int len, out byte[] data)
        {
            // There is no checksum in Modbus tcp, so the result is just a copy of the source data.
            data = new byte[arr.Length];
            if (data.Length > 0)
                Array.Copy(arr, data, data.Length);
            return true;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (slaveConnections.Count > 0)
            {
                foreach (var conn in slaveConnections)
                    conn.Dispose();
                slaveConnections.Clear();
            }
        }

        #endregion

    }
}

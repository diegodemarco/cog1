using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using static cog1.Literals.CommonLiterals;

namespace cog1.Modbus
{
    public class TcpSlave : IDisposable
    {
        private readonly int port;
        private readonly string host;
        private Socket socket = null;
        private Stopwatch sw = Stopwatch.StartNew();
        private long expiration = 0;

        public TcpSlave(string host)
        {
            if (!TryParse(host, out this.host, out this.port))
                throw new Exception($"Invalid host name {host}");
            KickExpiration();
        }

        public bool Connected => (socket != null) && socket.Connected;

        public bool IsHost(string host)
        {
            return TryParse(host, out var hn, out var port)
                && string.Equals(hn, this.host, StringComparison.OrdinalIgnoreCase)
                && port == this.port;
        }

        public bool Expired => (sw.ElapsedMilliseconds > expiration);

        public void KickExpiration()
        {
            expiration = sw.ElapsedMilliseconds + 60000;        // 1 minute
        }

        public static bool TryParse(string modbusHost, out string host, out int port)
        {
            if (modbusHost.Contains(':'))
            {
                var parts = modbusHost.Split(':');
                if (parts.Length != 2 || !UInt16.TryParse(parts[1], out var port16))
                {
                    host = string.Empty;
                    port = 0;
                    return false;
                }
                host = parts[0].Trim().ToLower();
                port = port16;
            }
            else
            {
                host = modbusHost.Trim().ToLower();
                port = 502;         // Modbus tcp default
            }
            return true;
        }

        public static bool IsValidModbusHost(string modbusHost)
        {
            return TryParse(modbusHost, out _, out _);
        }

        public Socket GetSocket(out string errorMessage)
        {
            if (socket == null)
            {
                //Console.WriteLine($"Connecting to {host}:{port}");
                socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                //Console.WriteLine($"Socket timeout: {socket.ReceiveTimeout}");
                try
                {
                    if (!socket.ConnectAsync(host, port).Wait(2000)) // This may fail
                    {
                        errorMessage = $"Connection to {host}:{port} failed";
                        return null;
                    }
                }
                catch 
                {
                    errorMessage = $"Connection to {host}:{port} failed";
                    return null;
                }
                //Console.WriteLine($"Connected. Got stream");
            }
            if (Connected)
            {
                errorMessage = string.Empty;
                return socket;
            }
            errorMessage = $"Operation ignored: will retry connection to {host}:{port} in {(int)((expiration - sw.ElapsedMilliseconds) / 1000)} seconds";
            return null;
        }

        public void Dispose()
        {
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
        }
    }
}

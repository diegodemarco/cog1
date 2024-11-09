using System.Diagnostics;
using System.IO.Ports;

namespace rtu_monitor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var port = new SerialPort("com5")
            {
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None
            };
            port.Open();

            var inFrame = false;
            var buffer = new byte[4096];
            var sw = Stopwatch.StartNew();
            long lastFrame = sw.ElapsedMilliseconds;
            for (; ; )
            {
                if (port.BytesToRead > 0)
                {
                    var readBytes = port.Read(buffer, 0, port.BytesToRead);
                    if (readBytes > 0)
                    {
                        ShowBytes(buffer, readBytes);
                        inFrame = true;
                        lastFrame = sw.ElapsedMilliseconds;
                    }
                }
                else
                {
                    if (inFrame && (sw.ElapsedMilliseconds - lastFrame) > 5)
                    {
                        inFrame = false;
                        Console.WriteLine();
                    }
                    Thread.Sleep(1);
                }
            }

        }

        private static void ShowBytes(byte[] buffer, int len)
        {
            Console.Write(BitConverter.ToString(buffer, 0, len).Replace("-", " ") + " ");
        }
    }
}

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using cog1.Display.Menu;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace cog1.Hardware
{
    public enum BootReason
    {
        PowerLoss = 0,
        Reboot = 1,
    }

    public enum EncoderEventType
    {
        None = 0,
        Right = 1,
        Left = 2,
        ButtonDown = 3,
        ButtonUp = 4,
    }

    public static partial class IOManager
    {
        private static object _lock = new object();

        #region Init & deinit

        public static bool Init()
        {
            if (active)
                return true;

            if (Global.IsDevelopment)
            {
                Console.WriteLine("Development mode: skipping io library initialization");
            }
            else
            {
                int ret;
                lock (_lock)
                {
                    // IO ISR
                    var del = new IOISRDelegate(EncoderISR);
                    p_fnISISR = Marshal.GetFunctionPointerForDelegate(del);
                    gchIOISR = GCHandle.Alloc(del);

                    // Initialize the IO library
                    ret = ioLib.io_init(p_fnISISR);
                    if (ret == 0)
                    {
                        Console.WriteLine("iolib.so init successful. Boot reason: power loss");
                        bootReason = BootReason.PowerLoss;
                        active = true;
                    }
                    else if (ret == 1)
                    {
                        Console.WriteLine("iolib.so init successful. Boot reason: reboot");
                        bootReason = BootReason.Reboot;
                        active = true;
                    }
                    else if (ret > 0)
                    {
                        Console.WriteLine($"iolib.so init successful. Boot reason: unknown ({ret})");
                        bootReason = BootReason.Reboot;
                        active = true;
                    }
                    else
                    {
                        Console.WriteLine($"iolib.so init error ({ret})");
                        return false;
                    }
                }
            }

            // Initial hardware reads
            AnalogRead();

            // We're good
            return true;
        }

        public static void Deinit()
        {
            if (!Global.IsDevelopment)
            {
                lock (_lock)
                {
                    if (OSUtils.Rebooting)
                    {
                        var canvas = new DisplayCanvas();
                        canvas.DrawText(0, 18, DisplayCanvas.Font_8x12, "   Rebooting");
                        canvas.DrawText(0, 37, DisplayCanvas.Font_6x8, "   Please wait...");
                        canvas.ToDisplay();
                    }
                    else
                    {
                        ioLib.display_clear();
                    }

                    if (active)
                    {
                        active = false;
                        ioLib.io_deinit();
                        Console.WriteLine("iolib.so deinit successful");
                    }
                }
            }
        }

        #endregion

        #region General properties

        private static bool active = false;
        private static BootReason bootReason = BootReason.PowerLoss;

        public static bool IsActive { get { lock (_lock) return active; } }

        public static BootReason BootReason => bootReason;

        #endregion

        #region IO library marshalling

        private const int IO_EVENT_ENCODER_CW = 1;
        private const int IO_EVENT_ENCODER_CCW = 2;
        private const int IO_EVENT_ENCODER_SW_ACTIVE = 4;
        private const int IO_EVENT_ENCODER_SW_INACTIVE = 8;
        private const int IO_EVENT_DI1_ACTIVE = 16;
        private const int IO_EVENT_DI1_INACTIVE = 32;
        private const int IO_EVENT_DI2_ACTIVE = 64;
        private const int IO_EVENT_DI2_INACTIVE = 128;
        private const int IO_EVENT_DI3_ACTIVE = 256;
        private const int IO_EVENT_DI3_INACTIVE = 512;
        private const int IO_EVENT_DI4_ACTIVE = 1024;
        private const int IO_EVENT_DI4_INACTIVE = 2048;

        private const int IO_EVENT_ENCODER =
            IO_EVENT_ENCODER_CW | IO_EVENT_ENCODER_CCW | IO_EVENT_ENCODER_SW_ACTIVE | IO_EVENT_ENCODER_SW_INACTIVE;
        private const int IO_EVENT_DI =
            IO_EVENT_DI1_ACTIVE | IO_EVENT_DI1_INACTIVE | IO_EVENT_DI2_ACTIVE | IO_EVENT_DI2_INACTIVE |
            IO_EVENT_DI3_ACTIVE | IO_EVENT_DI3_INACTIVE | IO_EVENT_DI4_ACTIVE | IO_EVENT_DI4_INACTIVE;

        private static nint p_fnISISR;
        private static GCHandle gchIOISR;
        private delegate void IOISRDelegate(int eventBitmap);

        private class ioLib
        {
            [DllImport("cog1_io.so")]
            public static extern int io_init(nint io_isr);
            [DllImport("cog1_io.so")]
            public static extern int io_deinit();
            [DllImport("cog1_io.so")]
            public static extern int heartbeat(int value);
            [DllImport("cog1_io.so")]
            public static extern int do_control(int bitmap);
            [DllImport("cog1_io.so")]
            public static extern int adc_read(ref int c1, ref int c2, ref int c3, ref int c4, ref int c5, ref int c6, ref int c7, ref int c8);
            [DllImport("cog1_io.so")]
            public static extern int display_init();
            [DllImport("cog1_io.so")]
            public static extern void display_clear();
            [DllImport("cog1_io.so")]
            public static extern void display_goto_xy(int x, int y);
            [DllImport("cog1_io.so")]
            public static extern void display_draw_bitmap(int x, int y, byte[] data, int data_len);
        }

        #endregion

        #region Digital input ISR

        private static void EncoderISR(int eventBitmap)
        {
            // Encoder events
            if ((eventBitmap & IO_EVENT_ENCODER) != 0)
            {
                // Encoder-related events
                if ((eventBitmap & IO_EVENT_ENCODER_CW) != 0)
                    DisplayMenu.EncoderEvent(EncoderEventType.Right);
                if ((eventBitmap & IO_EVENT_ENCODER_CCW) != 0)
                    DisplayMenu.EncoderEvent(EncoderEventType.Left);
                if ((eventBitmap & IO_EVENT_ENCODER_SW_ACTIVE) != 0)
                    DisplayMenu.EncoderEvent(EncoderEventType.ButtonDown);
                if ((eventBitmap & IO_EVENT_ENCODER_SW_INACTIVE) != 0)
                    DisplayMenu.EncoderEvent(EncoderEventType.ButtonUp);
            }

            // Digital input events
            if ((eventBitmap & IO_EVENT_DI) != 0)
            {
                lock (_lock)
                {
                    if ((eventBitmap & IO_EVENT_DI1_ACTIVE) != 0) _input_shadow[0] = true;
                    if ((eventBitmap & IO_EVENT_DI1_INACTIVE) != 0) _input_shadow[0] = false;
                    if ((eventBitmap & IO_EVENT_DI2_ACTIVE) != 0) _input_shadow[1] = true;
                    if ((eventBitmap & IO_EVENT_DI2_INACTIVE) != 0) _input_shadow[1] = false;
                    if ((eventBitmap & IO_EVENT_DI3_ACTIVE) != 0) _input_shadow[2] = true;
                    if ((eventBitmap & IO_EVENT_DI3_INACTIVE) != 0) _input_shadow[2] = false;
                    if ((eventBitmap & IO_EVENT_DI4_ACTIVE) != 0) _input_shadow[3] = true;
                    if ((eventBitmap & IO_EVENT_DI4_INACTIVE) != 0) _input_shadow[3] = false;
                }
            }

        }

        #endregion

        #region Display

        private static bool display_init = false;

        private static void CheckDisplayInit()
        {
            if (display_init)
                return;
            display_init = ioLib.display_init() != 0;
        }

        public static void Display_Draw(int x, int y, byte[] data)
        {
            lock (_lock)
            {
                if (!active)
                    return;
                CheckDisplayInit();
                ioLib.display_draw_bitmap(x, y, data ?? Array.Empty<byte>(), data == null ? 0 : data.Length);
            }
        }

        #endregion

        #region Digital inputs

        private static bool[] _input_shadow = { false, false, false, false };

        public static void ReadDI(out bool di1, out bool di2, out bool di3, out bool di4)
        {
            lock (_lock)
            {
                di1 = _input_shadow[0];
                di2 = _input_shadow[1];
                di3 = _input_shadow[2];
                di4 = _input_shadow[3];
            }
        }

        #endregion

        #region Digital outputs

        private static bool[] _output_shadow = { false, false, false, false };

        public static void ReadDO(out bool do1, out bool do2, out bool do3, out bool do4)
        {
            lock (_lock)
            {
                do1 = _output_shadow[0];
                do2 = _output_shadow[1];
                do3 = _output_shadow[2];
                do4 = _output_shadow[3];
            }
        }

        public static bool SetDigitalOutput(int output_number, bool value)
        {
            if (output_number < 1 || output_number > _output_shadow.Length)
                return false;
            lock (_lock)
            {
                _output_shadow[output_number - 1] = value;
                int bitmap = 0;
                if (_output_shadow[0]) bitmap += 0b00000001;
                if (_output_shadow[1]) bitmap += 0b10000010;
                if (_output_shadow[2]) bitmap += 0b10000100;
                if (_output_shadow[3]) bitmap += 0b10001000;
                ioLib.do_control(bitmap);
            }
            return true;
        }

        #endregion

        #region Analog inputs

        private static int _ana1_shadow = 0, _ana2_shadow = 0, _ana3_shadow = 0, _ana4_shadow = 0;
        private static int _anv1_shadow = 0, _anv2_shadow = 0, _anv3_shadow = 0, _anv4_shadow = 0;

        public static void Read020mA(out double ana1, out double ana2, out double ana3, out double ana4)
        {
            // adc is 12 bits
            const double adc_full_range = 4095;         // 12-bit adc
            const double adc_voltage_reference = 2.56;  // 2.56V

            // Resistor values used at the input of the adc
            const double shunt_resistor = 255;
            const double series_resistor = 10000;
            const double ground_resistor = 9530;
            const double resistor_divider_factor = ground_resistor / (series_resistor + ground_resistor);
            const double final_shunt_resistor = 1 / (1 / shunt_resistor + 1 / (series_resistor + ground_resistor));

            // Voltage at the input when the reads its maximum value
            const double adc_full_scale_voltage = adc_voltage_reference / resistor_divider_factor;

            double a1, a2, a3, a4;
            lock (_lock)
            {
                a1 = _ana1_shadow;
                a2 = _ana2_shadow;
                a3 = _ana3_shadow;
                a4 = _ana4_shadow;
            }

            // 
            ana1 = 1000 * (double)a1 * adc_full_scale_voltage / adc_full_range / final_shunt_resistor;
            ana2 = 1000 * (double)a2 * adc_full_scale_voltage / adc_full_range / final_shunt_resistor;
            ana3 = 1000 * (double)a3 * adc_full_scale_voltage / adc_full_range / final_shunt_resistor;
            ana4 = 1000 * (double)a4 * adc_full_scale_voltage / adc_full_range / final_shunt_resistor;
        }

        public static void Read010V(out double anv1, out double anv2, out double anv3, out double anv4)
        {
            // adc is 12 bits
            const double adc_full_range = 4095;         // 12-bit adc
            const double adc_voltage_reference = 2.56;  // 2.56V

            // Resistor values used at the input of the adc
            const double series_resistor = 30000;
            const double ground_resistor = 10000;
            const double resistor_divider_factor = ground_resistor / (series_resistor + ground_resistor);

            // Voltage at the input when the reads its maximum value
            const double adc_full_scale_voltage = adc_voltage_reference / resistor_divider_factor;

            double a1, a2, a3, a4;
            lock (_lock)
            {
                a1 = _anv1_shadow;
                a2 = _anv2_shadow;
                a3 = _anv3_shadow;
                a4 = _anv4_shadow;
            }

            // 
            anv1 = (double)a1 * adc_full_scale_voltage / adc_full_range;
            anv2 = (double)a2 * adc_full_scale_voltage / adc_full_range;
            anv3 = (double)a3 * adc_full_scale_voltage / adc_full_range;
            anv4 = (double)a4 * adc_full_scale_voltage / adc_full_range;
        }

        // Analog input polling

        private static void AnalogRead()
        {
            if (Global.IsDevelopment)
                return;

            int c0 = 0, c1 = 0, c2 = 0, c3 = 0, c4 = 0, c5 = 0, c6 = 0, c7 = 0;

            lock (_lock)
            {
                // Console.WriteLine("Polling analog inputs");
                if (ioLib.adc_read(ref c0, ref c1, ref c2, ref c3, ref c4, ref c5, ref c6, ref c7) != 0)
                {
                    _ana1_shadow = c3;
                    _ana2_shadow = c2;
                    _ana3_shadow = c1;
                    _ana4_shadow = c0;
                    _anv1_shadow = c7;
                    _anv2_shadow = c6;
                    _anv3_shadow = c5;
                    _anv4_shadow = c4;

                    //Console.WriteLine($"4-20mA read: {_ana1_shadow}, {_ana2_shadow}, {_ana3_shadow}, {_ana4_shadow}");
                    //Console.WriteLine($"0-10V read: {_anv1_shadow}, {_anv2_shadow}, {_anv3_shadow}, {_anv4_shadow}");
                }
                else
                {
                    Console.WriteLine("adc_read() failed");
                }
            }
        }

        private static void AnalogIOPoll()
        {
            for (; ; )
            {
                Thread.Sleep(1000);
                AnalogRead();
            }
        }

        #endregion

    }
}

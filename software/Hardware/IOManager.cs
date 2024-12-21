using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using cog1.Display.Menu;
using cog1.DTO;
using cog1.Entities;
using Newtonsoft.Json;

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
        private static bool persistNeeded = false;
        private static string VARIABLE_VALUES_FILE = Path.Combine(Global.DataDirectory, "variable_values.json");
        private static Dictionary<int, VariableValueDTO> variableValues = new();

        #region Variable IDs

        // Digital inputs
        public const int DI1_VARIABLE_ID = 1;
        public const int DI2_VARIABLE_ID = 2;
        public const int DI3_VARIABLE_ID = 3;
        public const int DI4_VARIABLE_ID = 4;

        public const int AV1_VARIABLE_ID = 5;
        public const int AV2_VARIABLE_ID = 6;
        public const int AV3_VARIABLE_ID = 7;
        public const int AV4_VARIABLE_ID = 8;

        public const int AC1_VARIABLE_ID = 9;
        public const int AC2_VARIABLE_ID = 10;
        public const int AC3_VARIABLE_ID = 11;
        public const int AC4_VARIABLE_ID = 12;

        public const int DO1_VARIABLE_ID = 13;
        public const int DO2_VARIABLE_ID = 14;
        public const int DO3_VARIABLE_ID = 15;
        public const int DO4_VARIABLE_ID = 16;

        #endregion

        #region Init & deinit

        public static bool Init()
        {
            if (active)
                return true;

            // Load variable values from disk
            DeserializeVariableValues();

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

            // Update digital outputs based on their startup configuration
            UpdateOutputOnStartup(Config.DO1StartupType, DO1_VARIABLE_ID, ref _output_shadow[0]);
            UpdateOutputOnStartup(Config.DO2StartupType, DO2_VARIABLE_ID, ref _output_shadow[1]);
            UpdateOutputOnStartup(Config.DO3StartupType, DO3_VARIABLE_ID, ref _output_shadow[2]);
            UpdateOutputOnStartup(Config.DO4StartupType, DO4_VARIABLE_ID, ref _output_shadow[3]);
            InternalUpdateDigitalOutputs();

            // Update the state of digital inputs by forcing a read from the hardware
            InternalUpdateDigitalInputs();

            // Force an initial read of the ADC inputs
            AnalogRead();

            // Launch data persistence task
            Task.Run(DataPersistTask);

            // Done
            return true;
        }

        public static void Deinit()
        {
            // Persist data before shutting down
            PersistData();

            if (!Global.IsDevelopment)
            {
                lock (_lock)
                {
                    DisplayCanvas canvas;
                    switch (OSUtils.RebootStatus)
                    {
                        case RebootStatus.Reinit:
                            canvas = new DisplayCanvas();
                            canvas.DrawText(0, 18, DisplayCanvas.Font_8x12, "Re-initializing");
                            canvas.DrawText(0, 37, DisplayCanvas.Font_6x8,  "   Please wait...");
                            canvas.ToDisplay();
                            break;
                        case RebootStatus.Reboot:
                            canvas = new DisplayCanvas();
                            canvas.DrawText(0, 18, DisplayCanvas.Font_8x12, "   Rebooting");
                            canvas.DrawText(0, 37, DisplayCanvas.Font_6x8, "   Please wait...");
                            canvas.ToDisplay();
                            break;
                        default:
                            ioLib.display_clear();
                            break;
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

        private static bool GetVariableValue(int variable_id, out double? value, out DateTime? lastUpdateUtc)
        {
            lock (_lock)
            {
                if (variableValues.TryGetValue(variable_id, out var entry))
                {
                    value = entry.value;
                    lastUpdateUtc = entry.lastUpdateUtc;
                    return true;
                }
                value = null;
                lastUpdateUtc = null;
                return false;
            }
        }

        private static void DeserializeVariableValues()
        {
            lock (_lock)
            {
                if (File.Exists(VARIABLE_VALUES_FILE))
                {
                    variableValues = JsonConvert.DeserializeObject<Dictionary<int, VariableValueDTO>>(File.ReadAllText(VARIABLE_VALUES_FILE));
                }
                else
                {
                    variableValues = new();
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
            public static extern int di_read(ref int bitmap);
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
                    if ((eventBitmap & IO_EVENT_DI1_ACTIVE) != 0)
                    {
                        _input_shadow[0] = true;
                        SetVariableValue(DI1_VARIABLE_ID, 1);
                    }
                    if ((eventBitmap & IO_EVENT_DI1_INACTIVE) != 0)
                    {
                        _input_shadow[0] = false;
                        SetVariableValue(DI1_VARIABLE_ID, 0);
                    }
                    if ((eventBitmap & IO_EVENT_DI2_ACTIVE) != 0)
                    {
                        _input_shadow[1] = true;
                        SetVariableValue(DI2_VARIABLE_ID, 1);
                    }
                    if ((eventBitmap & IO_EVENT_DI2_INACTIVE) != 0)
                    {
                        _input_shadow[1] = false;
                        SetVariableValue(DI2_VARIABLE_ID, 0);
                    }
                    if ((eventBitmap & IO_EVENT_DI3_ACTIVE) != 0)
                    {
                        _input_shadow[2] = true;
                        SetVariableValue(DI3_VARIABLE_ID, 1);
                    }
                    if ((eventBitmap & IO_EVENT_DI3_INACTIVE) != 0)
                    {
                        _input_shadow[2] = false;
                        SetVariableValue(DI3_VARIABLE_ID, 0);
                    }
                    if ((eventBitmap & IO_EVENT_DI4_ACTIVE) != 0)
                    {
                        _input_shadow[3] = true;
                        SetVariableValue(DI4_VARIABLE_ID, 1);
                    }
                    if ((eventBitmap & IO_EVENT_DI4_INACTIVE) != 0)
                    {
                        _input_shadow[3] = false;
                        SetVariableValue(DI4_VARIABLE_ID, 0);
                    }
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

        private static void InternalUpdateDigitalInputs()
        {
            if (Global.IsDevelopment)
                return;

            lock (_lock)
            {
                // Read inputs
                int value = 0;
                ioLib.di_read(ref value);

                // Update shadow values
                _input_shadow[0] = (value & 1) > 0;
                _input_shadow[1] = (value & 2) > 0;
                _input_shadow[2] = (value & 4) > 0;
                _input_shadow[3] = (value & 8) > 0;

                // Update the respective variables
                EnsureVariableValue(DI1_VARIABLE_ID, _input_shadow[0] ? 1 : 0);
                EnsureVariableValue(DI2_VARIABLE_ID, _input_shadow[1] ? 1 : 0);
                EnsureVariableValue(DI3_VARIABLE_ID, _input_shadow[2] ? 1 : 0);
                EnsureVariableValue(DI4_VARIABLE_ID, _input_shadow[3] ? 1 : 0);

                Console.WriteLine($"Synchronized digital inputs to [{_input_shadow[0]}, {_input_shadow[1]}, {_input_shadow[2]}, {_input_shadow[3]}]");
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

        private static void UpdateOutputOnStartup(OutputStartupType startType, int variableId, ref bool shadowValue)
        {
            switch (startType)
            {
                case OutputStartupType.On:
                    // On
                    shadowValue = true;
                    break;

                case OutputStartupType.Restore:
                    // Restore
                    shadowValue = GetVariableValue(variableId, out var value, out _) && value != null && value != 0;
                    break;

                default:
                    // Off
                    shadowValue = false;
                    break;
            }
            EnsureVariableValue(variableId, shadowValue ? 1 : 0);
        }

        private static void InternalUpdateDigitalOutputs()
        {
            if (Global.IsDevelopment)
                return;

            lock (_lock)
            {
                int bitmap = 0;
                if (_output_shadow[0]) bitmap += 0b00000001;
                if (_output_shadow[1]) bitmap += 0b10000010;
                if (_output_shadow[2]) bitmap += 0b10000100;
                if (_output_shadow[3]) bitmap += 0b10001000;
                ioLib.do_control(bitmap);
            }
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

        #region Variable values

        public static bool SetVariableValue(int variableId, double value)
        {
            lock (_lock)
            {
                switch (variableId)
                {
                    case DO1_VARIABLE_ID:
                        _output_shadow[0] = (value != 0);
                        InternalUpdateDigitalOutputs();
                        break;
                    case DO2_VARIABLE_ID:
                        _output_shadow[1] = (value != 0);
                        InternalUpdateDigitalOutputs();
                        break;
                    case DO3_VARIABLE_ID:
                        _output_shadow[2] = (value != 0);
                        InternalUpdateDigitalOutputs();
                        break;
                    case DO4_VARIABLE_ID:
                        _output_shadow[3] = (value != 0);
                        InternalUpdateDigitalOutputs();
                        break;
                }
                variableValues[variableId] = new VariableValueDTO()
                {
                    variableId = variableId,
                    value = value,
                    lastUpdateUtc = DateTime.UtcNow
                };
                PersistNeeded();
                return true;
            }
        }

        public static Dictionary<int, VariableValueDTO> GetVariableValues()
        {
            lock (_lock)
            {
                return variableValues.Values.ToDictionary(item => item.variableId);
            }
        }

        private static double? GetVariableValue(int variableId)
        {
            lock (_lock)
            {
                if (variableValues.TryGetValue(variableId, out var v))
                    return v.value;
                return null;
            }
        }

        private static void EnsureVariableValue(int variableId, double value)
        {
            if (GetVariableValue(variableId) != value)
                SetVariableValue(variableId, value);
        }

        public static void UpdateVariableList(HashSet<int> varIds)
        {
            lock (_lock)
            {
                var idsToRemove = variableValues.Select(item => item.Key).Where(item => !varIds.Contains(item));
                foreach (var item in idsToRemove)
                    variableValues.Remove(item);
            }
        }

        #endregion

        #region Data persistence

        private static void PersistNeeded()
        {
            lock (_lock)
            {
                persistNeeded = true;
            }
        }

        private static void PersistData()
        {
            lock (_lock)
            {
                if (persistNeeded) 
                {
                    File.WriteAllText(VARIABLE_VALUES_FILE, JsonConvert.SerializeObject(variableValues));
                    Console.WriteLine("Persisted variable data");
                    persistNeeded = false;
                }
            }
        }

        private static void DataPersistTask()
        {
            for (; ; )
            {
                try
                {
                    Thread.Sleep(60000);        // Persist every 60 seconds
                    PersistData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DataPersisTask error: ", ex.ToString());
                    Thread.Sleep(1000);
                }
            }
        }

        #endregion

    }
}

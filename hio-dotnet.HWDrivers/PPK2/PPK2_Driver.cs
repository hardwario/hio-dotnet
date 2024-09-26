using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace hio_dotnet.HWDrivers.PPK2
{
    public class PPK2_Driver : IDisposable
    {
        private SerialPort _serialPort;
        private ConcurrentQueue<byte> _dataQueue; // Use ConcurrentQueue for thread-safe access
        private Dictionary<string, Dictionary<string, double>> modifiers;
        private bool isMeasuring;
        private int vddLow = 800;
        private int vddHigh = 5000;
        private int? currentVdd;
        private double adcMult = 1.8 / 163840;
        private string mode = null;
        private double? rollingAvg = null;
        private double? rollingAvg4 = null;
        private string prevRange = null;
        private int consecutiveRangeSamples = 0;
        private double spikeFilterAlpha = 0.18;
        private double spikeFilterAlpha5 = 0.06;
        private int spikeFilterSamples = 3;
        private int afterSpike = 0;

        public PPK2_Driver(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            _serialPort = new SerialPort(portName)
            {
                BaudRate = 9600, // Dummy value, won't affect USB CDC ACM communication
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.None, // Ensure no hardware flow control
                ReadTimeout = 1000, // Increase read timeout if needed
                WriteTimeout = 1000, // Increase write timeout if needed
                ReceivedBytesThreshold = 1 // Trigger DataReceived event as soon as data is available
            };

            _serialPort.DataReceived += SerialPort_DataReceived; // Attach event handler
            _dataQueue = new ConcurrentQueue<byte>(); // Initialize ConcurrentQueue
            isMeasuring = false;
            _serialPort.Open();

            modifiers = new Dictionary<string, Dictionary<string, double>>()
            {
                { "R", new Dictionary<string, double> { { "0", 1031.64 }, { "1", 101.65 }, { "2", 10.15 }, { "3", 0.94 }, { "4", 0.043 } } },
                { "GS", new Dictionary<string, double> { { "0", 1 }, { "1", 1 }, { "2", 1 }, { "3", 1 }, { "4", 1 } } },
                { "GI", new Dictionary<string, double> { { "0", 1 }, { "1", 1 }, { "2", 1 }, { "3", 1 }, { "4", 1 } } },
                { "O", new Dictionary<string, double> { { "0", 0 }, { "1", 0 }, { "2", 0 }, { "3", 0 }, { "4", 0 } } },
                { "S", new Dictionary<string, double> { { "0", 0 }, { "1", 0 }, { "2", 0 }, { "3", 0 }, { "4", 0 } } },
                { "I", new Dictionary<string, double> { { "0", 0 }, { "1", 0 }, { "2", 0 }, { "3", 0 }, { "4", 0 } } },
                { "UG", new Dictionary<string, double> { { "0", 1 }, { "1", 1 }, { "2", 1 }, { "3", 1 }, { "4", 1 } } }
            };
        }

        private void ResetDevice()
        {
            WriteSerial(new byte[] { PPK2_Command.RESET });
        }

        private void WriteSerial(byte[] cmd)
        {
            try
            {
                _serialPort.Write(cmd, 0, cmd.Length);
                Console.WriteLine($"Sent command: {BitConverter.ToString(cmd)}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while writing to the serial port: {ex.Message}");
            }
        }

        private (byte, byte) ConvertSourceVoltage(int mV)
        {
            if (mV < vddLow) mV = vddLow;
            if (mV > vddHigh) mV = vddHigh;

            int offset = 32;
            int diffToBaseline = mV - vddLow + offset;

            byte baseB1 = 3;
            byte baseB2 = 0;

            byte setB1 = (byte)(baseB1 + (diffToBaseline / 256));
            byte setB2 = (byte)(baseB2 + (diffToBaseline % 256));

            return (setB1, setB2);
        }

        /// <summary>
        /// Set voltage in milivolts
        /// </summary>
        /// <param name="mV"></param>
        public void SetSourceVoltage(int mV)
        {
            var (b1, b2) = ConvertSourceVoltage(mV);
            WriteSerial(new byte[] { PPK2_Command.REGULATOR_SET, b1, b2 });
            currentVdd = mV;
        }

        /// <summary>
        /// Turn on and off the power supply. Use "ON" for turn on and "OFF" for turn off the power supply.
        /// </summary>
        /// <param name="state"></param>
        public void ToggleDUTPower(PPK2_OutputState state)
        {
            if (state == PPK2_OutputState.ON)
            {
                WriteSerial(new byte[] { PPK2_Command.DEVICE_RUNNING_SET, PPK2_Command.TRIGGER_SET });
            }
            else if (state == PPK2_OutputState.OFF)
            {
                WriteSerial(new byte[] { PPK2_Command.DEVICE_RUNNING_SET, PPK2_Command.NO_OP });
            }
        }

        /// <summary>
        /// Use the ampere meter mode of the PPK2
        /// </summary>
        public void UseAmpereMeter()
        {
            mode = PPK2_Modes.AMPERE_MODE;
            WriteSerial(new byte[] { PPK2_Command.SET_POWER_MODE, PPK2_Command.TRIGGER_SET });
        }

        /// <summary>
        /// Use the source meter mode of the PPK2
        /// </summary>
        public void UseSourceMeter()
        {
            mode = PPK2_Modes.SOURCE_MODE;
            WriteSerial(new byte[] { PPK2_Command.SET_POWER_MODE, PPK2_Command.AVG_NUM_SET });
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Check if data is available
                int bytesToRead = _serialPort.BytesToRead;
                if (bytesToRead > 0)
                {
                    byte[] buffer = new byte[bytesToRead];
                    int bytesRead = _serialPort.Read(buffer, 0, bytesToRead);

                    if (bytesRead > 0)
                    {
                        foreach (var b in buffer)
                        {
                            _dataQueue.Enqueue(b);
                        }

                        // Display received data
                        Console.WriteLine($"Received Data: {BitConverter.ToString(buffer)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while receiving data: {ex.Message}");
            }
        }

        /// <summary>
        /// Dequeue data from the SerialPort buffer and return it as a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] GetData()
        {
            List<byte> dataList = new List<byte>();

            while (_dataQueue.TryDequeue(out byte dataByte))
            {
                dataList.Add(dataByte);
            }

            return dataList.ToArray();
        }

        /// <summary>
        /// Start measurement 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void StartMeasuring()
        {
            if (!currentVdd.HasValue)
            {
                if (mode == PPK2_Modes.SOURCE_MODE)
                {
                    throw new InvalidOperationException("Output voltage not set!");
                }
                if (mode == PPK2_Modes.AMPERE_MODE)
                {
                    throw new InvalidOperationException("Input voltage not set!");
                }
            }

            // Set the measurement mode again for safety
            if (mode == PPK2_Modes.SOURCE_MODE)
            {
                UseSourceMeter();
            }
            else if (mode == PPK2_Modes.AMPERE_MODE)
            {
                UseAmpereMeter();
            }

            WriteSerial(new byte[] { PPK2_Command.AVERAGE_START });
            Console.WriteLine("Measurement started.");
            isMeasuring = true;
        }

        /// <summary>
        /// Stop measurement
        /// </summary>
        public void StopMeasuring()
        {
            WriteSerial(new byte[] { PPK2_Command.AVERAGE_STOP });
            Console.WriteLine("Measurement stopped.");
            isMeasuring = false;
        }


        /// <summary>
        /// Dispose device
        /// </summary>
        public void Dispose()
        {
            StopMeasuring();
            ResetDevice();
            _serialPort.DataReceived -= SerialPort_DataReceived; // Detach event handler
            _serialPort?.Close();
        }
    }
}

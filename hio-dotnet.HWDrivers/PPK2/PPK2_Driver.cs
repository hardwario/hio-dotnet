using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
#if LIBSERIALPORT
using hio_dotnet.LibSerialPort;
#else
using System.IO.Ports;
#endif

namespace hio_dotnet.HWDrivers.PPK2
{
    public class PPK2_Driver : IDisposable
    {
#if LIBSERIALPORT
        private LibSerialPortDriver _serialPort;
#else
        private SerialPort _serialPort;
#endif
        private ConcurrentQueue<byte> _dataQueue; // Use ConcurrentQueue for thread-safe access
        private Dictionary<string, Dictionary<string, double>> modifiers;
        private bool isMeasuring = false;
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

        // Masks and shifts for extract of the measured values samples
        private const uint MEAS_ADC_MASK = 0x00003FFF; // Bits 0-13
        private const int MEAS_ADC_SHIFT = 0;

        private const uint MEAS_RANGE_MASK = 0x0001C000; // Bits 14-16
        private const int MEAS_RANGE_SHIFT = 14;

        private const uint MEAS_LOGIC_MASK = 0xFF000000; // Bits 24-31
        private const int MEAS_LOGIC_SHIFT = 24;

        // Buffer for remainder from previous read, if data is not multiple of 4
        private byte[] remainderBuffer = new byte[0];

#if LIBSERIALPORT
        public PPK2_Driver(string portName, int baudRate = 9600, SpParity parity = SpParity.SP_PARITY_NONE, int dataBits = 8, SpStopBits stopBits = SpStopBits.SP_STOP_BITS_ONE)
        {
            _dataQueue = new ConcurrentQueue<byte>(); // Initialize ConcurrentQueue

            isMeasuring = false;
            
            _serialPort = new LibSerialPortDriver();
            _serialPort.DataReceived += SerialPort_DataReceived; // Attach event handler
            _serialPort.OpenPort(portName, baudRate, dataBits, parity, stopBits);
            _serialPort.SetRTS(true);
            _serialPort.SetDTR(true);

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
#else
        public PPK2_Driver(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            _dataQueue = new ConcurrentQueue<byte>(); // Initialize ConcurrentQueue

            _serialPort = new SerialPort(portName)
            {
                BaudRate = 9600, // Dummy value, won't affect USB CDC ACM communication
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.None, // Ensure no hardware flow control
                ReadTimeout = 1000, // Increase read timeout if needed
                WriteTimeout = 1000, // Increase write timeout if needed
                ReceivedBytesThreshold = 1, // Trigger DataReceived event as soon as data is available
                DtrEnable = true,
                RtsEnable = true
            };

            _serialPort.DataReceived += SerialPort_DataReceived;

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
#endif

        private void ResetDevice()
        {
            WriteSerial(new byte[] { PPK2_Command.RESET });
        }

        private void WriteSerial(byte[] cmd)
        {
            try
            {
#if LIBSERIALPORT
                _serialPort.Write(cmd);
#else
                _serialPort.Write(cmd, 0, cmd.Length);
#endif
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

#if LIBSERIALPORT
        private void SerialPort_DataReceived(object sender, byte[] e)
        {
            try
            {
                if (e.Length > 0)
                {
                    foreach (var b in e)
                    {
                        _dataQueue.Enqueue(b);
                    }

                    // Display received data
                    //Console.WriteLine($"Received Data: {BitConverter.ToString(buffer)}");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while receiving data: {ex.Message}");
            }
        }
#else
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
                    }

                    // Display received data
                    //Console.WriteLine($"Received Data: {BitConverter.ToString(buffer)}");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while receiving data: {ex.Message}");
            }
        }
#endif

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
        /// Process acumulated data from the measurement
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<double> ProcessData(byte[] data)
        {
            List<double> samples = new List<double>();
            List<int> digitalBits = new List<int>();

            // Connect last data with new data
            byte[] buffer = remainderBuffer.Concat(data).ToArray();

            int offset = 0;
            int sampleSize = 4;

            while (offset + sampleSize <= buffer.Length)
            {
                byte[] sampleBytes = new byte[sampleSize];
                Array.Copy(buffer, offset, sampleBytes, 0, sampleSize);
                offset += sampleSize;

                // Convert to 32bit unsigned integer (little-endian)
                uint sampleValue = BitConverter.ToUInt32(sampleBytes, 0);

                // Extract ADC result
                uint adcResult = (sampleValue & MEAS_ADC_MASK) >> MEAS_ADC_SHIFT;
                adcResult *= 4; // Multiply 4, same as original driver

                // Extract measurement range
                uint measurementRange = (sampleValue & MEAS_RANGE_MASK) >> MEAS_RANGE_SHIFT;
                measurementRange = Math.Min(measurementRange, 4); // Max 4

                // Extract logic bits
                uint bits = (sampleValue & MEAS_LOGIC_MASK) >> MEAS_LOGIC_SHIFT;

                // Recalc to analog value in μA
                double analogValue = GetADCResult(measurementRange.ToString(), adcResult);

                // Add to the sample list
                samples.Add(analogValue);

                // save digital bits if necessary
                digitalBits.Add((int)bits);
            }

            // Save for next iteration
            remainderBuffer = buffer.Skip(offset).ToArray();

            return samples;
        }

        private double GetADCResult(string currentRange, uint adcValue)
        {
            // Get Calibration constants
            double R = modifiers["R"][currentRange];
            double GS = modifiers["GS"][currentRange];
            double GI = modifiers["GI"][currentRange];
            double O = modifiers["O"][currentRange];
            double S = modifiers["S"][currentRange];
            double I = modifiers["I"][currentRange];
            double UG = modifiers["UG"][currentRange];

            // Calculate gain
            double resultWithoutGain = (adcValue - O) * (adcMult / R);

            // Get Analog value
            double adc = UG * (resultWithoutGain * (GS * resultWithoutGain + GI) + (S * (currentVdd.Value / 1000.0) + I));

            // Apply filter if necessary
            adc = ApplySpikeFilter(currentRange, adc);

            // convert to microampers (μA)
            return adc * 1e6;
        }

        private double ApplySpikeFilter(string currentRange, double adc)
        {
            double prevRollingAvg = rollingAvg ?? adc;
            double prevRollingAvg4 = rollingAvg4 ?? adc;

            // Spike filtering / rolling average
            if (rollingAvg == null)
            {
                rollingAvg = adc;
            }
            else
            {
                rollingAvg = spikeFilterAlpha * adc + (1 - spikeFilterAlpha) * rollingAvg;
            }

            if (rollingAvg4 == null)
            {
                rollingAvg4 = adc;
            }
            else
            {
                rollingAvg4 = spikeFilterAlpha5 * adc + (1 - spikeFilterAlpha5) * rollingAvg4;
            }

            if (prevRange == null)
            {
                prevRange = currentRange;
            }

            if (prevRange != currentRange || afterSpike > 0)
            {
                if (prevRange != currentRange)
                {
                    consecutiveRangeSamples = 0;
                    afterSpike = spikeFilterSamples;
                }
                else
                {
                    consecutiveRangeSamples += 1;
                }

                if (currentRange == "4")
                {
                    if (consecutiveRangeSamples < 2)
                    {
                        rollingAvg = prevRollingAvg;
                        rollingAvg4 = prevRollingAvg4;
                    }
                    adc = rollingAvg4.Value;
                }
                else
                {
                    adc = rollingAvg.Value;
                }

                afterSpike -= 1;
            }

            prevRange = currentRange;
            return adc;
        }

        /// <summary>
        /// Start capturing of the values and save to the file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="durationMilliseconds"></param>
        /// <returns></returns>
        public async Task CaptureMeasurement(string filename, int durationMilliseconds, bool addTimestampPrefix = false, bool useDotForDecimals = true)
        {
            if (addTimestampPrefix || File.Exists(filename))
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_");
                filename = timestamp + filename;
            }

            Console.WriteLine($"Starting Measurement with file output to {filename}.");
            StartMeasuring();

            // Set time limit with auto cancel
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(durationMilliseconds);

            // PPK2 should has 100kHz sampling frequency based on docs
            double samplingFrequency = 100000; // 100 kHz
            double samplingInterval = 1.0 / samplingFrequency; // in secodns!!!
            double currentTime = 0.0;

            var numberFormat = useDotForDecimals ? new CultureInfo("en-US") : new CultureInfo("cs-CZ");

            using (StreamWriter writer = new StreamWriter(filename))
            {
                // Create file header
                writer.WriteLine("Time (s),Current (μA)");

                try
                {
                    // Measurement start
                    DateTime startTime = DateTime.Now;

                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        byte[] data = GetData();

                        if (data.Length > 0)
                        {
                            List<double> newSamples = ProcessData(data);

                            foreach (var sample in newSamples)
                            {
                                // culture invariant version. it uses . in double on any culture environment
                                //writer.WriteLine($"{currentTime.ToString(System.Globalization.CultureInfo.InvariantCulture)};{sample.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

                                // this version is based on useDotForDecimals input parameter
                                string timeStr = currentTime.ToString(numberFormat);
                                string sampleStr = sample.ToString(numberFormat);

                                writer.WriteLine($"{timeStr};{sampleStr}");

                                currentTime += samplingInterval;
                            }
                        }

                        await Task.Delay(10);
                    }
                }
                finally
                {
                    StopMeasuring();
                }
            }

            Console.WriteLine($"Measurement finished. Data saved in {filename} file.");
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

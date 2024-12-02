using hio_dotnet.HWDrivers.Enums;
using hio_dotnet.HWDrivers.JLink;
using hio_dotnet.HWDrivers.MCU;
using hio_dotnet.HWDrivers.PPK2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Demos.HardwarioMonitor.Services
{
    public class DataPoint
    {
        public long Time { get; set; } = 0;
        public int Current { get; set; } = 0;
    }

    public class ConsoleService
    {
        public MCUMultiRTTConsole? MCUConsole { get; set; }
        public PPK2_Driver? ppk2 { get; set; }

        private CancellationTokenSource cts = new CancellationTokenSource();

        public event EventHandler<Tuple<string, MultiRTTClientBase>?> NewRTTMessageLineReceived;

        public event EventHandler<DataPoint[]?> DataPointsReceived;

        private const int dataPointsLength = 20000;
        private const int latestDataPointsLength = 2000;
        public DataPoint[] dataPoints { get; set; } = new DataPoint[dataPointsLength];
        public DataPoint[] latestDataPoints { get; set; } = new DataPoint[latestDataPointsLength];
        public int DataPointsIndex { get; set; } = 0;
        public int DataPointsTimeSinceStartCounter { get; set; } = 0;

        public bool IsConsoleListening { get; set; } = false;

        public bool IsPPK2Connected()
        {
            return ppk2 != null;
        }

        public bool IsDeviceOn { get; set; } = false;
        public int DeviceVoltage { get; set; } = 0;

        public void InitArrays()
        {
            for (int i = 0; i < dataPoints.Length; i++)
            {
                dataPoints[i] = new DataPoint() { Time = i, Current = 0 };
            }

            for (int i = 0; i < latestDataPoints.Length; i++)
            {
                latestDataPoints[i] = new DataPoint() { Time = i, Current = 0 };
            }
        }

        public async Task FindAndConnectPPK()
        {
            var devices = PPK2_DeviceManager.ListAvailablePPK2Devices();

            if (devices.Count == 0)
            {
                Console.WriteLine("No PPK2 devices found. Exiting program...");
                Console.WriteLine("Program ends. Goodbye. Press any key to quit...");
                Console.ReadLine();
                return;
            }

            var selectedDevice = devices[0];
            Console.WriteLine($"\nUsing PPK2 device on COM Port: {selectedDevice.PortName} with Serial Number: {selectedDevice.SerialNumber}");

            ppk2 = new PPK2_Driver(selectedDevice.PortName);

        }

        public async Task SetPPK2Voltage(int voltage)
        {
            // Set a source voltage (e.g., 3300 mV)
            //int voltage = 3300;
            Console.WriteLine($"Setting source voltage to {voltage} mV...");
            ppk2?.SetSourceVoltage(voltage);
            DeviceVoltage = voltage;
        }

        public async Task TurnOnPower(bool withMeasurement = false)
        {
            // Turn on the DUT power
            Console.WriteLine("Turning on DUT power...");
            ppk2?.ToggleDUTPower(PPK2_OutputState.ON);
            IsDeviceOn = true;
            if (withMeasurement)
                await MeasureLoop();
        }

        public async Task TurnOffPower()
        {
            if (IsConsoleListening)
                await StopListening();

            Console.WriteLine("Turning off DUT power...");
            ppk2?.ToggleDUTPower(PPK2_OutputState.OFF);
            IsDeviceOn = false;
        }

        public async Task MeasureLoop()
        {
            if (ppk2 != null)
            {
                new Task(async () =>
                {
                    // Start measuring
                    Console.WriteLine("Starting measurement...");
                    ppk2.StartMeasuring();
                    long totalstartTime = (long)(DateTime.Now - DateTime.MinValue).TotalMilliseconds;
                    DataPointsTimeSinceStartCounter = 0;

                    var canceled = false;
                    while (!canceled)
                    {
                        // Collect data for X seconds
                        var ontime = 2;
                        Console.WriteLine($"Collecting data for {ontime} seconds...");
                        DateTime startTime = DateTime.Now;
                        while ((DateTime.Now - startTime).TotalSeconds < ontime)
                        {
                            byte[] data = ppk2.GetData();

                            if (data.Length > 0)
                            {
                                //var counter = 0;

                                var downsampledData = Downsample(ppk2.ProcessData(data).ToArray(), 100000, 1000);

                                //dataPoints = new DataPoint[downsampledData.Length];

                                foreach (var sample in downsampledData)
                                {
                                    if (DataPointsIndex < dataPointsLength)
                                    {
                                        //Console.WriteLine($"Current: {sample} μA");
                                        //calculate start time of the datapoint in ms if the sampling frequency is 100khz
                                        var time = ((startTime - DateTime.MinValue).TotalMilliseconds - totalstartTime) + (DataPointsTimeSinceStartCounter * 100);
                                        //dataPoints[DataPointsIndex] = new DataPoint() { Time = (long)time, Current = (int)sample };
                                        dataPoints[DataPointsIndex].Time = (long)time;
                                        dataPoints[DataPointsIndex].Current = (int)sample;

                                        DataPointsIndex++;
                                        DataPointsTimeSinceStartCounter++;
                                    }
                                    else
                                    {
                                        DataPointsIndex = 0;
                                    }
                                }

                            }

                            await Task.Delay(50);

                            if (cts.Token.IsCancellationRequested)
                            {
                                canceled = true;
                                break;
                            }
                        }
                        /*
                        CopyLastDataPoints();
                        for (int i = 0; i < latestDataPointsLength; i++)
                        {
                            latestDataPoints[i].Time = i;
                        }
                        */
                        DataPointsReceived?.Invoke(this, null);

                        if (cts.Token.IsCancellationRequested)
                        {
                            // Stop measuring
                            Console.WriteLine("Stopping measurement...");
                            ppk2.StopMeasuring();
                            await Task.Delay(1000);
                            canceled = true;
                            break;
                        }
                    }
                }).Start();
            }
        }

        public void CopyLastDataPoints()
        {
            if (DataPointsIndex >= latestDataPointsLength)
            {
                Array.Copy(dataPoints, DataPointsIndex - latestDataPointsLength, latestDataPoints, 0, latestDataPointsLength);
            }
            else
            {
                int tailLength = dataPointsLength - (latestDataPointsLength - DataPointsIndex);
                Array.Copy(dataPoints, tailLength, latestDataPoints, 0, latestDataPointsLength - DataPointsIndex);
                Array.Copy(dataPoints, 0, latestDataPoints, latestDataPointsLength - DataPointsIndex, DataPointsIndex);
            }
        }

        public async Task StartListening()
        {
            // Get all available connected JLinks
            Console.WriteLine("Searching for available JLinks...");
            var connected_jlinks = JLinkDriver.GetConnectedJLinks();
            if (connected_jlinks == null)
            {
                Console.WriteLine("Cannot find any JLinks.");
                Console.WriteLine("Program ends. Goodbye. Press any key to quit...");
                Console.ReadLine();
                return;
            }

            var numofjlinks = connected_jlinks?.Where(j => j.SerialNumber != 0).Count();
            if (numofjlinks == 0)
            {
                Console.WriteLine("Cannot find any JLinks.");
                Console.WriteLine("Program ends. Goodbye. Press any key to quit...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"{numofjlinks} JLinks found.");

            for (var i = 0; i < numofjlinks; i++)
            {
                Console.WriteLine($"SN: {connected_jlinks[i].SerialNumber}, Product: {connected_jlinks[i].acProduct}, NickName: {connected_jlinks[i].acNickName}");
            }

            // Take first available JLink
            var devsn = connected_jlinks[0].SerialNumber.ToString();

            if (ppk2 == null)
            {
                await FindAndConnectPPK();
                await SetPPK2Voltage(3300);
                await TurnOnPower();
            }

            // Create MCUConsole instances for Config and Log RTT channels
            Console.WriteLine("JLink RTT Console is Starting :)\n\n");

            MCUConsole = new MCUMultiRTTConsole(new List<MultiRTTClientBase>()
                {
                    new MultiRTTClientBase(){ Channel = 0, DriverType= RTTDriverType.JLinkRTT, Name = "ConfigConsole" },
                    new MultiRTTClientBase(){ Channel = 1, DriverType= RTTDriverType.JLinkRTT, Name = "LogConsole" }
                }, "nRF52840_xxAA", 4000, "mcumulticonsole", devsn);

            // Subscribe to NewRTTMessageLineReceived event to get RTT messages and set output to console
            MCUConsole.NewRTTMessageLineReceived += (sender, data) => NewRTTMessageLineReceived.Invoke(sender, data);

            var cts = new CancellationTokenSource();
            Task listeningTask = MCUConsole.StartListening(cts.Token);

            IsConsoleListening = true;

            await Task.WhenAny(new Task[] { listeningTask });

        }
        public async Task SendCommand(string command)
        {
            await MCUConsole?.SendCommand(0, command);
        }

        public async Task StopListening()
        {
            cts.Cancel();

            IsConsoleListening = false;

            MCUConsole?.Dispose();

            Console.WriteLine("Turning off DUT power...");
            ppk2?.ToggleDUTPower(PPK2_OutputState.OFF);
        }

        public double[] Downsample(double[] data, int originalRate, int targetRate)
        {
            if (originalRate < targetRate)
            {
                throw new ArgumentException("Cílová vzorkovací frekvence musí být nižší než původní.");
            }

            int ratio = originalRate / targetRate;

            if (data.Length % ratio != 0)
            {
                data = data.Take(data.Length - (data.Length % ratio)).ToArray();
            }

            int newLength = data.Length / ratio;
            double[] downsampledData = new double[newLength];

            for (int i = 0; i < newLength; i++)
            {
                downsampledData[i] = data.Skip(i * ratio).Take(ratio).Average();
            }

            return downsampledData;
        }
    }
}

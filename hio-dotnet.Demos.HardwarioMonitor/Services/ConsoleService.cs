using DocumentFormat.OpenXml.Office2010.CustomUI;
using hio_dotnet.Common.Config;
using hio_dotnet.HWDrivers.Enums;
using hio_dotnet.HWDrivers.JLink;
using hio_dotnet.HWDrivers.MCU;
using hio_dotnet.HWDrivers.PPK2;
using hio_dotnet.HWDrivers.Server;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.DataCollection;
using Radzen;
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
        private readonly NotificationService _notificationService;
        public ConsoleService(NotificationService notificationService)
        {
            _notificationService = notificationService;
            remoteWebSocketClient = new RemoteWebSocketClient();
        }

        public MCUMultiRTTConsole? MCUConsole { get; set; }
        public PPK2_Driver? ppk2 { get; set; }

        private RemoteWebSocketClient? remoteWebSocketClient;

        private CancellationTokenSource cts = new CancellationTokenSource();

        public event EventHandler<Tuple<string, MultiRTTClientBase>?> NewRTTMessageLineReceived;

        public event EventHandler<DataPoint[]?> DataPointsReceived;

        public event EventHandler<bool> OnIsBusy;
        public event EventHandler<bool> OnIsPPKConnected;
        public event EventHandler<bool> OnIsPPKDisconnected;

        public event EventHandler<bool> OnIsPPKVoltageOutputConnected;
        public event EventHandler<bool> OnIsPPKVoltageOutputDisconnected;

        public event EventHandler<bool> OnIsJLinkConnected;
        public event EventHandler<bool> OnIsJLinkDisconnected;

        public event EventHandler<List<ZephyrRTOSCommand>> OnHintForConsoleRefreshed;

        public List<string> ConsoleOutputShell = new List<string>();
        public List<string> ConsoleOutputLog = new List<string>();

        private const int dataPointsLength = 20000;
        private const int latestDataPointsLength = 2000;
        public DataPoint[] dataPoints { get; set; } = new DataPoint[dataPointsLength];
        public DataPoint[] latestDataPoints { get; set; } = new DataPoint[latestDataPointsLength];
        public int DataPointsIndex { get; set; } = 0;
        public int DataPointsTimeSinceStartCounter { get; set; } = 0;

        public bool IsConsoleListening { get; set; } = false;
        public bool IsPPK2Connected { get => ppk2 != null; }

        public bool IsDeviceOn { get; set; } = false;
        public int DeviceVoltage { get; set; } = 0;

        public bool UseRemoteConnection { get; set; } = false;
        public bool IsHostOfRemoteConnection { get; set; } = false;

        public LoRaWANConfig LoRaWANConfig { get; set; } = new LoRaWANConfig();
        public LTEConfig LTEConfig { get; set; } = new LTEConfig();

        public Guid RemoteSessionId { get => remoteWebSocketClient?.SessionId ?? Guid.Empty; }

        private void ShowNotification(NotificationMessage message)
        {
            message.Style = MainDataContext.NotificationPosition;
            _notificationService.Notify(message);
        }

        public async Task<bool> ConnectToRemoteSession(Guid sessionId)
        {
            if (remoteWebSocketClient != null)
            {
                try
                {
                    remoteWebSocketClient.SessionId = sessionId;

                    remoteWebSocketClient.OnMessageReceived -= RemoteWebSocketClient_OnMessageReceived;
                    remoteWebSocketClient.OnMessageReceived += RemoteWebSocketClient_OnMessageReceived;
                    remoteWebSocketClient.OnCommandReceived -= RemoteWebSocketClient_OnCommandReceived;
                    remoteWebSocketClient.OnCommandReceived += RemoteWebSocketClient_OnCommandReceived;

                    StartRemoteWSListening();
                    await Task.Delay(100);

                    await remoteWebSocketClient.SendWSSessionMessage("Registering as user in the session.");

                    UseRemoteConnection = true;
                    IsHostOfRemoteConnection = true;
                    return true;
                }
                catch
                {
                    ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Remote Server Error", Detail = "Remote Server is not running.", Duration = 3000 });
                }
            }
            return false;
        }

        public async Task StartRemoteWSListening()
        {
            if (remoteWebSocketClient != null)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await remoteWebSocketClient.ConnectAsync("wss://thingsboard.hardwario.com/ws");
                    }
                    catch
                    {
                        ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Remote Server Error", Detail = "Remote Server is not running.", Duration = 3000 });
                    }
                });
            }
        }

        public async Task<bool> StartRemoteSession(string login, string password)
        {
            if (remoteWebSocketClient != null)
            {
                try
                {
                    await remoteWebSocketClient.LoginToWSForwarderServer(login, password);
                    await remoteWebSocketClient.OpenSessionOnWSForwarderServer();

                    remoteWebSocketClient.OnMessageReceived -= RemoteWebSocketClient_OnMessageReceived;
                    remoteWebSocketClient.OnMessageReceived += RemoteWebSocketClient_OnMessageReceived;

                    StartRemoteWSListening();
                    await Task.Delay(100);

                    await remoteWebSocketClient.SendWSSessionMessage("Registering as host in the session.");

                    UseRemoteConnection = true;
                    IsHostOfRemoteConnection = false;
                    return true;
                }
                catch
                {
                    ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Remote Server Error", Detail = "Remote Server is not running.", Duration = 3000 });
                }
            }
            return false;
        }

        public async Task<bool> StopRemoteWSSession()
        {
            if (remoteWebSocketClient != null)
            {
                try
                {
                    await remoteWebSocketClient.DisconnectAsync();
                    remoteWebSocketClient.SessionId = Guid.Empty;
                    UseRemoteConnection = false;
                    IsHostOfRemoteConnection = false;
                    ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Remote Connection Closed", Detail = "Remote connection was closed.", Duration = 3000 });
                    return true;
                }
                catch
                {
                    ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Remote Server Error", Detail = "Remote Server is not running.", Duration = 3000 });
                }
            }
            return false;
        }

        private void RemoteWebSocketClient_OnCommandReceived(string obj)
        {
            if (!string.IsNullOrEmpty(obj) && UseRemoteConnection && IsHostOfRemoteConnection)
            {
                SendCommand(obj);
            }
        }

        private void RemoteWebSocketClient_OnMessageReceived(string obj)
        {
            try
            {
                var parsed = System.Text.Json.JsonSerializer.Deserialize<WebSocketModuleTransferObject>(obj);
                if (parsed != null)
                {
                    ProcessNewRTTLine(null, new Tuple<string, MultiRTTClientBase>(parsed.Message, parsed.ClientBase));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse message as Tuple<string, MultiRTTClientBase>: {ex.Message}");
            }
        }

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

        public void StopAll(object sender, EventArgs e)
        {
            if (IsConsoleListening)
            {
                StopListening();
            }
            if (IsPPK2Connected)
            {
                ppk2?.Dispose();
            }
        }

        public async Task FindAndConnectPPK(bool doNotTurnBusy = false)
        {
            MainDataContext.OnStopJLink -= StopAll;
            MainDataContext.OnStopJLink += StopAll;

            OnIsBusy?.Invoke(this, true);
            await Task.Delay(10);
            var devices = PPK2_DeviceManager.ListAvailablePPK2Devices();

            if (devices.Count == 0)
            {
                Console.WriteLine("No PPK2 devices found. Exiting program...");
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "PPK2 not found", Detail = "No PPK2 devices found.", Duration = 3000 });
                return;
            }

            var selectedDevice = devices[0];
            Console.WriteLine($"\nUsing PPK2 device on COM Port: {selectedDevice.PortName} with Serial Number: {selectedDevice.SerialNumber}");

            ppk2 = new PPK2_Driver(selectedDevice.PortName);

            OnIsPPKConnected?.Invoke(this, true);
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "PPK2 connected", Detail = "PPK2 is connected.", Duration = 3000 });
            if (!doNotTurnBusy)
            {
                OnIsBusy?.Invoke(this, false);
                await Task.Delay(10);
            }
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

            if (DeviceVoltage == 0)
                DeviceVoltage = 3600;

            OnIsPPKVoltageOutputConnected?.Invoke(this, true);
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Power On", Detail = "Power supply for device is on.", Duration = 3000 });
        }

        public async Task TurnOffPower()
        {
            if (IsConsoleListening)
                await StopListening();

            Console.WriteLine("Turning off DUT power...");
            ppk2?.ToggleDUTPower(PPK2_OutputState.OFF);
            IsDeviceOn = false;

            OnIsPPKVoltageOutputDisconnected?.Invoke(this, true);
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Power Off", Detail = "Power supply for device is off.", Duration = 3000 });
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

        public async Task StartListening(bool withPPK2 = true)
        {
            OnIsBusy?.Invoke(this, true);
            await Task.Delay(10);
            // Get all available connected JLinks
            Console.WriteLine("Searching for available JLinks...");
            var connected_jlinks = JLinkDriver.GetConnectedJLinks();
            if (connected_jlinks == null)
            {
                Console.WriteLine("Cannot find any JLinks.");
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "JLink not found", Detail = "Cannot find and JLink", Duration = 3000 });
                OnIsBusy?.Invoke(this, false);
                return;
            }

            var numofjlinks = connected_jlinks?.Where(j => j.SerialNumber != 0).Count();
            if (numofjlinks == 0)
            {
                Console.WriteLine("Cannot find any JLinks.");
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "JLink not found", Detail = "Cannot find and JLink", Duration = 3000 });
                OnIsBusy?.Invoke(this, false);
                return;
            }

            Console.WriteLine($"{numofjlinks} JLinks found.");

            for (var i = 0; i < numofjlinks; i++)
            {
                Console.WriteLine($"SN: {connected_jlinks[i].SerialNumber}, Product: {connected_jlinks[i].acProduct}, NickName: {connected_jlinks[i].acNickName}");
            }

            // Take first available JLink
            var devsn = connected_jlinks[0].SerialNumber.ToString();

            if (withPPK2 && ppk2 == null)
            {
                await FindAndConnectPPK(true);
                await SetPPK2Voltage(3600);
                await TurnOnPower();
            }

            // Create MCUConsole instances for Config and Log RTT channels
            Console.WriteLine("JLink RTT Console is Starting :)\n\n");

            try
            {
                MCUConsole = new MCUMultiRTTConsole(new List<MultiRTTClientBase>()
                {
                    new MultiRTTClientBase(){ Channel = 0, DriverType= RTTDriverType.JLinkRTT, Name = "ConfigConsole" },
                    new MultiRTTClientBase(){ Channel = 1, DriverType= RTTDriverType.JLinkRTT, Name = "LogConsole" }
                }, "nRF52840_xxAA", 4000, "mcumulticonsole", devsn);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while creating MCUConsole: {ex.Message}");
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = $"Error while creating MCUConsole: {ex.Message}.", Duration = 5000 });
                OnIsBusy?.Invoke(this, false);
                return;
            }
            // Subscribe to NewRTTMessageLineReceived event to get RTT messages and set output to console

            MCUConsole.NewRTTMessageLineReceived -= ProcessNewRTTLine;
            MCUConsole.NewRTTMessageLineReceived += ProcessNewRTTLine;
            MCUConsole.NewInternalCommandSent += MCUConsole_NewInternalCommandSent;

            cts = new CancellationTokenSource();
            Task listeningTask = MCUConsole.StartListening(cts.Token);

            IsConsoleListening = true;

            await Task.Delay(2000);

            OnIsBusy?.Invoke(this, false);
            await Task.Delay(10);
            OnIsJLinkConnected?.Invoke(this, true);
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "JLink Connected", Detail = "JLink is connected now.", Duration = 3000 });

            await Task.WhenAny(new Task[] { listeningTask });

        }

        private void MCUConsole_NewInternalCommandSent(object? sender, string e)
        {
            ConsoleOutputShell.Add(e);
        }

        private void ProcessNewRTTLine(object sender, Tuple<string, MultiRTTClientBase>? data)
        {
            if (UseRemoteConnection && IsHostOfRemoteConnection && data != null && data.Item1 != null && data.Item2 != null)
            {
                var json = System.Text.Json.JsonSerializer.Serialize(new WebSocketModuleTransferObject() { Message = data.Item1, ClientBase = data.Item2 });
                remoteWebSocketClient.SendWSSessionMessage($"{json}");
            }

            if (data?.Item2.Channel == 0)
            {
                var line = $"{data.Item1}";
                ConsoleOutputShell.Add(line);
                if (line.Contains("config"))
                {
                    if (line.Contains("lrw "))
                    {
                        LoRaWANConfig.ParseLine(line);
                    }
                    else if (line.Contains("lte "))
                    {
                        LTEConfig.ParseLine(line);
                    }
                }
            }
            else if (data?.Item2.Channel == 1)
            {
                var line = $"[{DateTime.UtcNow}][Log]: {data.Item1}";
                ConsoleOutputLog.Add(data.Item1);
            }

            NewRTTMessageLineReceived?.Invoke(sender, data);
        }

        public async Task SendCommand(string command)
        {
            if (IsConsoleListening || (UseRemoteConnection && !IsHostOfRemoteConnection))
            {
                ConsoleOutputShell.Add("> " + command.Replace("\n", string.Empty));

                if (command.Contains("config"))
                {
                    if (command.Contains("lrw "))
                    {
                        LoRaWANConfig.ParseLine(command);
                    }
                    else if (command.Contains("lte "))
                    {
                        LTEConfig.ParseLine(command);
                    }
                }

                if ((!UseRemoteConnection || (UseRemoteConnection && IsHostOfRemoteConnection)) && MCUConsole != null)
                {
                    try
                    {
                        await MCUConsole.SendCommand(0, command);
                    }
                    catch
                    {
                        ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Drivers Server Error", Detail = "Driver Server is not running.", Duration = 3000 });
                    }
                }
                else if (UseRemoteConnection && !IsHostOfRemoteConnection && remoteWebSocketClient != null)
                {
                    try
                    {
                        await remoteWebSocketClient.SendCommand(command);
                    }
                    catch
                    {
                        ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Remote Server Error", Detail = "Remote Server is not running.", Duration = 3000 });
                    }
                }
            }
            else
            {
                Console.WriteLine("Console is not listening. Cannot send command.");
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = "Console is not listening. Cannot send command.", Duration = 3000 });
            }
        }

        public async Task StopListening()
        {
            cts.Cancel();

            IsConsoleListening = false;

            MCUConsole?.Dispose();

            OnIsJLinkDisconnected?.Invoke(this, true);

            Console.WriteLine("Turning off DUT power...");
            ppk2?.ToggleDUTPower(PPK2_OutputState.OFF);
            OnIsPPKVoltageOutputDisconnected?.Invoke(this, true);
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "JLink disconnected", Detail = "JLink is disconnected and power is off.", Duration = 3000 });
        }

        public double[] Downsample(double[] data, int originalRate, int targetRate)
        {
            if (originalRate < targetRate)
            {
                throw new ArgumentException("Target frequency must be lower than the original.");
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

        public async Task SaveConsoleShellToFile()
        {
            var fileService = new FileService();
            await fileService.SaveFileWithDialogAsync(ConsoleOutputShell, "ConsoleShellOutput.txt");
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "File Saved", Detail = "Console content has been saved to the file.", Duration = 3000 });
        }

        public async Task SaveConsoleLogToFile()
        {
            var fileService = new FileService();
            await fileService.SaveFileWithDialogAsync(ConsoleOutputLog, "ConsoleLogOutput.txt");
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "File Saved", Detail = "Console Log content has been saved to the file.", Duration = 3000 });
        }

        private async Task SendAllConfigLines(string cfg)
        {
            var lines = new List<string>();
            if (!string.IsNullOrEmpty(cfg))
            {
                lines = cfg.Split("\n").ToList();
                foreach (var line in lines)
                {
                    await SendCommand(line);
                }
            }
        }

        public async Task ApplyLoRaSettings()
        {
            var cfg = LoRaWANConfig.GetWholeConfig();
            await SendAllConfigLines(cfg);
            await SendCommand("config save");
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Settings Applied", Detail = "LoRaWAN settings has been loaded to the device.", Duration = 3000 });
        }

        public async Task ApplyLTESettings()
        {
            var cfg = LTEConfig.GetWholeConfig();
            await SendAllConfigLines(cfg);
            await SendCommand("config save");
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Settings Applied", Detail = "LTE settings has been loaded to the device.", Duration = 3000 });
        }

        public async Task LoadCommandsFromDevice(string parent = "")
        {
            if (MCUConsole != null && IsConsoleListening)
            {
                OnIsBusy?.Invoke(this, true);
                var commands = await MCUConsole.LoadCommandsFromDeviceHelp(parent, 0);
                if (!string.IsNullOrEmpty(parent))
                {
                    foreach (var cmd in commands)
                    {
                        cmd.Command = parent + " " + cmd.Command;
                    }
                }
                ZephyrRTOSStandardCommands.StandardCommands.AddRange(commands);
                OnHintForConsoleRefreshed?.Invoke(this, commands);
                OnIsBusy?.Invoke(this, false);
            }
        }
        public async Task Dispose()
        {
            if (IsConsoleListening)
            {
                await StopListening();
            }
            ppk2?.Dispose();
            MCUConsole?.Dispose();
        }
    }
}

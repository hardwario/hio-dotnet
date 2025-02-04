using hio_dotnet.Common.Config;
using hio_dotnet.HWDrivers.Enums;
using hio_dotnet.HWDrivers.JLink;
using hio_dotnet.HWDrivers.MCU;
using hio_dotnet.HWDrivers.PPK2;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.DataCollection;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hio_dotnet.HWDrivers.Server;

namespace hio_dotnet.Demos.BlazorComponents.RadzenLib.Services
{

    public class RemoteConsoleService
    {
        private readonly NotificationService _notificationService;
        public RemoteConsoleService(NotificationService notificationService)
        {
            _notificationService = notificationService;
            driversServerApiClient = new DriversServerApiClient(null);
            driversWebSocketClient = new DriversWebSocketClient();

            driversWebSocketClient.OnMessageReceived += DriversWebSocketClient_OnMessageReceived;           
        }

        private DriversServerApiClient? driversServerApiClient;
        private DriversWebSocketClient? driversWebSocketClient;

        private CancellationTokenSource cts = new CancellationTokenSource();

        public event EventHandler<Tuple<string, MultiRTTClientBase>?> NewRTTMessageLineReceived;

        public event EventHandler<bool> OnIsBusy;
        public event EventHandler<bool> OnIsPPKConnected;
        public event EventHandler<bool> OnIsPPKDisconnected;

        public event EventHandler<bool> OnIsPPKVoltageOutputConnected;
        public event EventHandler<bool> OnIsPPKVoltageOutputDisconnected;

        public event EventHandler<bool> OnIsJLinkConnected;
        public event EventHandler<bool> OnIsJLinkDisconnected;

        public List<string> ConsoleOutputShell = new List<string>();
        public List<string> ConsoleOutputLog = new List<string>();

        public bool IsConsoleListening { get; set; } = false;
        public bool IsPPK2Connected { get; set; } = false;
        public async Task<bool> IsPPK2ConnectedAsync()
        {
            if (driversServerApiClient != null)
            {
                var result = driversServerApiClient.PPK2_DeviceStatus().Result;
                return result;
            }
            return false;               
        }

        public bool IsDeviceOn { get; set; } = false;
        public int DeviceVoltage { get; set; } = 0;

        public LoRaWANConfig LoRaWANConfig { get; set; } = new LoRaWANConfig();
        public LTEConfig LTEConfig { get; set; } = new LTEConfig();

        private void DriversWebSocketClient_OnMessageReceived(string obj)
        {
            try
            {
                var parsed = System.Text.Json.JsonSerializer.Deserialize<Tuple<string, MultiRTTClientBase>>(obj);
                if (parsed != null)
                {
                    ProcessNewRTTLine(null, parsed);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse message as Tuple<string, MultiRTTClientBase>: {ex.Message}");
            }
        }

        private void ShowNotification(NotificationMessage message)
        {
            message.Style = MainDataContext.NotificationPosition;
            _notificationService.Notify(message);
        }

        public void StopAll(object sender, EventArgs e)
        {
            if (IsConsoleListening)
            {
                StopListening();
            }
            if (IsPPK2Connected)
            {
                if (driversServerApiClient != null)
                    driversServerApiClient.PPK2_TurnOff().Wait();
            }
        }

        public async Task FindAndConnectPPK(bool doNotTurnBusy = false)
        {
            if (driversServerApiClient == null)
                driversServerApiClient = new DriversServerApiClient(null);
            
            OnIsBusy?.Invoke(this, true);
            await Task.Delay(10);

            var devices = await driversServerApiClient.PPK2_GetPortsNames();

            if (devices.Count == 0)
            {
                Console.WriteLine("No PPK2 devices found. Exiting program...");
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "PPK2 not found", Detail = "No PPK2 devices found.", Duration = 3000 });
                return;
            }

            var selectedDevice = devices[0];
            Console.WriteLine($"\nUsing PPK2 device on COM Port: {selectedDevice.PortName} with Serial Number: {selectedDevice.SerialNumber}");

            await driversServerApiClient.PPK2_Init(selectedDevice.PortName);
            IsPPK2Connected = true;

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
            if (driversServerApiClient != null)
                await driversServerApiClient.PPK2_SetVoltage(voltage);

            DeviceVoltage = voltage;
        }

        public async Task TurnOnPower(bool withMeasurement = false)
        {
            // Turn on the DUT power
            Console.WriteLine("Turning on DUT power...");
            if (driversServerApiClient != null)
                await driversServerApiClient.PPK2_TurnOn();

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
            if (driversServerApiClient != null)
                await driversServerApiClient.PPK2_TurnOff();

            IsDeviceOn = false;

            OnIsPPKVoltageOutputDisconnected?.Invoke(this, true);
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Power Off", Detail = "Power supply for device is off.", Duration = 3000 });
        }

        public async Task MeasureLoop()
        {
            return;
        }

        public async Task StartListening()
        {
            OnIsBusy?.Invoke(this, true);
            await Task.Delay(10);

            

            if (driversServerApiClient != null)
                await driversServerApiClient.JLink_Init();

            IsConsoleListening = true;

            await Task.Delay(2000);

            OnIsBusy?.Invoke(this, false);
            await Task.Delay(10);
            OnIsJLinkConnected?.Invoke(this, true);
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "JLink Connected", Detail = "JLink is connected now.", Duration = 3000 });

            if (driversWebSocketClient != null)
            {
                _ = Task.Run(async () =>
                {
                    await driversWebSocketClient.ConnectAsync("ws://localhost:8042/ws");
                });
            }
        }

        private void ProcessNewRTTLine(object sender, Tuple<string, MultiRTTClientBase>? data)
        {
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
            if (IsConsoleListening)
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

                if (driversServerApiClient != null)
                    await driversServerApiClient.JLink_SendCommandByName("ConfigConsole", command);
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

            if (driversServerApiClient != null)
                await driversServerApiClient.JLink_Stop();

            OnIsJLinkDisconnected?.Invoke(this, true);

            Console.WriteLine("Turning off DUT power...");

            await TurnOffPower();
            OnIsPPKVoltageOutputDisconnected?.Invoke(this, true);
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "JLink disconnected", Detail = "JLink is disconnected and power is off.", Duration = 3000 });
        }


        public async Task SaveConsoleShellToFile()
        {
            /*
            var fileService = new FileService();
            await fileService.SaveFileWithDialogAsync(ConsoleOutputShell, "ConsoleShellOutput.txt");
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "File Saved", Detail = "Console content has been saved to the file.", Duration = 3000 });
            */
        }

        public async Task SaveConsoleLogToFile()
        {
            /*
            var fileService = new FileService();
            await fileService.SaveFileWithDialogAsync(ConsoleOutputLog, "ConsoleLogOutput.txt");
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "File Saved", Detail = "Console Log content has been saved to the file.", Duration = 3000 });
            */
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

        public async Task Dispose()
        {
            if (IsConsoleListening)
            {
                await StopListening();
            }
        }
    }
}

﻿using hio_dotnet.Common.Config;
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
using Microsoft.JSInterop;
using hio_dotnet.HWDrivers.Server;

namespace hio_dotnet.Demos.BlazorComponents.RadzenLib.Services
{

    public class RemoteConsoleService
    {
        private readonly NotificationService _notificationService;
        private readonly IJSRuntime _jsRuntime;

        public RemoteConsoleService(NotificationService notificationService, IJSRuntime jsRuntime)
        {
            _notificationService = notificationService;
            driversServerApiClient = new DriversServerApiClient(null);
            driversWebSocketClient = new DriversWebSocketClient();
            remoteWebSocketClient = new RemoteWebSocketClient();

            driversWebSocketClient.OnMessageReceived += DriversWebSocketClient_OnMessageReceived;
            _jsRuntime = jsRuntime;
            StartWSListening();
        }

        private DriversServerApiClient? driversServerApiClient;
        private DriversWebSocketClient? driversWebSocketClient;
        private RemoteWebSocketClient? remoteWebSocketClient;

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
        public bool UseRemoteConnection { get; set; } = false;
        public bool IsHostOfRemoteConnection { get; set; } = false;

        public Guid RemoteSessionId { get => remoteWebSocketClient?.SessionId ?? Guid.Empty; }

        public async Task StartWSListening()
        {
            if (driversWebSocketClient != null)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await driversWebSocketClient.ConnectAsync("ws://localhost:8042/ws");
                    }
                    catch
                    {
                        ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Drivers Server Error", Detail = "Driver Server is not running.", Duration = 3000 });
                    }
                });
            }
        }

        public async Task StartRemoteWSListening()
        {
            if (remoteWebSocketClient != null)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await remoteWebSocketClient.ConnectAsync("ws://localhost:5000/ws");
                    }
                    catch
                    {
                        ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Remote Server Error", Detail = "Remote Server is not running.", Duration = 3000 });
                    }
                });
            }
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

                    await remoteWebSocketClient.SendWSSessionMessage("Registering as host in the session.");

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

                    await remoteWebSocketClient.SendWSSessionMessage("Registering as user in the session.");

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
            if (UseRemoteConnection && !IsHostOfRemoteConnection)
                DriversWebSocketClient_OnMessageReceived(obj);
        }

        public async Task<bool> IsPPK2ConnectedAsync()
        {
            if (driversWebSocketClient != null)
            {
                try
                {
                    var result = driversWebSocketClient.PPK2_DeviceStatus().Result;
                    return result;
                }
                catch
                {
                    ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Drivers Server Error", Detail = "Driver Server is not running.", Duration = 3000 });
                }
            }
            return false;               
        }

        public bool IsDeviceOn { get; set; } = false;
        public int DeviceVoltage { get; set; } = 0;

        public LoRaWANConfig LoRaWANConfig { get; set; } = new LoRaWANConfig();
        public LTEConfig LTEConfig { get; set; } = new LTEConfig();

        public async Task GetStatuses()
        {
            if (driversWebSocketClient != null)
            {
                try
                {
                    IsDeviceOn = await driversWebSocketClient.PPK2_DeviceStatus();
                    if (IsDeviceOn)
                    {
                        IsPPK2Connected = true;
                    }

                    DeviceVoltage = await driversWebSocketClient.PPK2_DeviceVoltage();
                }
                catch
                {
                    ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Drivers Server Error", Detail = "Driver Server is not running.", Duration = 3000 });
                }
            }
        }

        private void DriversWebSocketClient_OnMessageReceived(string obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj))
                    return;

                if (UseRemoteConnection && IsHostOfRemoteConnection)
                {
                    remoteWebSocketClient.SendWSSessionMessage($"{obj}");
                }
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
                if (driversWebSocketClient != null)
                {
                    try
                    {
                        driversWebSocketClient.PPK2_TurnOff().Wait();
                    }
                    catch
                    {
                        ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Drivers Server Error", Detail = "Driver Server is not running.", Duration = 3000 });
                    }
                }
            }
        }

        public async Task FindAndConnectPPK(bool doNotTurnBusy = false)
        {
            if (driversWebSocketClient == null)
                driversWebSocketClient = new DriversWebSocketClient();
            
            OnIsBusy?.Invoke(this, true);
            await Task.Delay(10);

            var devices = await driversWebSocketClient.PPK2_GetPortsNames();

            if (devices.Count == 0)
            {
                Console.WriteLine("No PPK2 devices found. Exiting program...");
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "PPK2 not found", Detail = "No PPK2 devices found.", Duration = 3000 });
                return;
            }

            var selectedDevice = devices.Where(n => n.PortName.Contains("ACM0")).FirstOrDefault();
            if (selectedDevice == null)
            {
                selectedDevice = devices.Where(n => n.PortName.Contains("COM")).FirstOrDefault() ?? new GetPortsResponse();
            }

            Console.WriteLine($"\nUsing PPK2 device on COM Port: {selectedDevice.PortName} with Serial Number: {selectedDevice.SerialNumber}");

            selectedDevice.PortName = selectedDevice.PortName.Replace("\\/", "/").Replace("/", "%2F");
            
            try
            {
                await driversWebSocketClient.PPK2_Init(selectedDevice.PortName);
            }
            catch
            {
                ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Drivers Server Error", Detail = "Driver Server is not running.", Duration = 3000 });
            }
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
            if (driversWebSocketClient != null)
            {
                try
                {
                    await driversWebSocketClient.PPK2_SetVoltage(voltage);
                }
                catch
                {
                    ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Drivers Server Error", Detail = "Driver Server is not running.", Duration = 3000 });
                }
            }

            DeviceVoltage = voltage;
        }

        public async Task TurnOnPower(bool withMeasurement = false)
        {
            // Turn on the DUT power
            Console.WriteLine("Turning on DUT power...");
            if (driversWebSocketClient != null)
            {
                try
                {
                    await driversWebSocketClient.PPK2_TurnOn();
                }
                catch
                {
                    ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Drivers Server Error", Detail = "Driver Server is not running.", Duration = 3000 });
                }
            }

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
            if (driversWebSocketClient != null)
            {
                try 
                { 
                    await driversWebSocketClient.PPK2_TurnOff();
                }
                catch
                {
                    ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Drivers Server Error", Detail = "Driver Server is not running.", Duration = 3000 });
                }
            }

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

            if (driversWebSocketClient != null)
            {
                try
                {
                    await driversWebSocketClient.JLink_Init();
                }
                catch
                {
                    ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Drivers Server Error", Detail = "Driver Server is not running.", Duration = 3000 });
                }
            }

            IsConsoleListening = true;

            await Task.Delay(2000);

            OnIsBusy?.Invoke(this, false);
            await Task.Delay(10);
            OnIsJLinkConnected?.Invoke(this, true);
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "JLink Connected", Detail = "JLink is connected now.", Duration = 3000 });

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

                if ((!UseRemoteConnection || (UseRemoteConnection && IsHostOfRemoteConnection)) && driversWebSocketClient != null)
                {
                    try
                    {
                        await driversWebSocketClient.JLink_SendCommandByName("ConfigConsole", command);
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

            if (driversWebSocketClient != null)
            {
                try
                {
                    await driversWebSocketClient.JLink_Stop();
                    IsConsoleListening = false;
                }
                catch
                {
                    ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Drivers Server Error", Detail = "Driver Server is not running.", Duration = 3000 });
                }
            }

            OnIsJLinkDisconnected?.Invoke(this, true);

            Console.WriteLine("Turning off DUT power...");

            await TurnOffPower();
            OnIsPPKVoltageOutputDisconnected?.Invoke(this, true);
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "JLink disconnected", Detail = "JLink is disconnected and power is off.", Duration = 3000 });
        }


        public async Task SaveConsoleShellToFile()
        {
            var filename = $"{DateTime.Now:yyyyMMddHHmmss}_ConsoleShellOutput.txt";
            await _jsRuntime.InvokeVoidAsync("window.hiodotnet.downloadText", ConsoleOutputShell, filename);
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "File Saved", Detail = "Console content has been saved to the file.", Duration = 3000 });
            
        }

        public async Task SaveConsoleLogToFile()
        {
            var filename = $"{DateTime.Now:yyyyMMddHHmmss}_ConsoleLogOutput.txt";
            await _jsRuntime.InvokeVoidAsync("window.hiodotnet.downloadText", ConsoleOutputLog, filename);
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

        public async Task Dispose()
        {
            if (IsConsoleListening)
            {
                await StopListening();
            }
        }
    }
}

using EmbedIO.Routing;
using EmbedIO;
using hio_dotnet.HWDrivers.Enums;
using hio_dotnet.HWDrivers.JLink;
using hio_dotnet.HWDrivers.MCU;
using hio_dotnet.HWDrivers.PPK2;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hio_dotnet.APIs.HioCloud;

namespace hio_dotnet.HWDrivers.Server
{
    public class DriversCommonController
    {
        /// <summary>
        /// Get available ports for PPK2
        /// </summary>
        /// <returns></returns>
        public List<GetPortsResponse> PPK2_GetPortNames()
        {
            var res = new List<GetPortsResponse>();
            var r = PPK2_DeviceManager.ListAvailablePPK2Devices();
            foreach (var item in r)
            {
                res.Add(new GetPortsResponse() { PortName = item.PortName, SerialNumber = item.SerialNumber });
            }

            return res;
        }

        /// <summary>
        /// Init PPK2 driver
        /// </summary>
        /// <param name="portname"></param>
        /// <returns></returns>
        public string PPK2_Init(string portname)
        {
            DriversServerMainDataContext.PPK2_Driver = new PPK2_Driver(portname);

            return "OK";
        }

        /// <summary>
        /// Set PPK2 output voltage in milivolts
        /// </summary>
        /// <param name="voltage">voltage in milivolts</param>
        /// <returns></returns>
        public string PPK2_SetVoltage(int voltage)
        {
            if (DriversServerMainDataContext.PPK2_Driver == null)
                return "PPK2 driver not initialized";

            DriversServerMainDataContext.PPK2_Driver.SetSourceVoltage(voltage);

            return "OK";
        }

        /// <summary>
        /// Turn on PPK2 output
        /// </summary>
        /// <returns></returns>
        public string PPK2_TurnOn()
        {
            if (DriversServerMainDataContext.PPK2_Driver == null)
                return "PPK2 driver not initialized";

            Console.WriteLine("Turning on DUT power...");

            DriversServerMainDataContext.PPK2_Driver.ToggleDUTPower(PPK2_OutputState.ON);

            DriversServerMainDataContext.IsDeviceOn = true;

            if (DriversServerMainDataContext.DeviceVoltage == 0)
                DriversServerMainDataContext.DeviceVoltage = 3600;
            return "OK";
        }

        /// <summary>
        /// Turn off PPK2 output
        /// </summary>
        /// <returns></returns>
        public string PPK2_TurnOff()
        {
            if (DriversServerMainDataContext.PPK2_Driver == null)
                return "PPK2 driver not initialized";

            Console.WriteLine("Turning off DUT power...");
            DriversServerMainDataContext.PPK2_Driver.ToggleDUTPower(PPK2_OutputState.OFF);
            DriversServerMainDataContext.IsDeviceOn = false;

            return "OK";
        }

        /// <summary>
        /// Get actual PPK2 device status. It means if the device is on or off
        /// </summary>
        /// <returns></returns>
        public bool PPK2_DeviceStatus()
        {
            return DriversServerMainDataContext.IsDeviceOn;
        }

        /// <summary>
        /// Get actual PPK2 device voltage
        /// </summary>
        /// <returns></returns>
        public int PPK2_DeviceVoltage()
        {
            return DriversServerMainDataContext.DeviceVoltage;
        }

        /// <summary>
        /// Init JLink RTT Console
        /// If PPK2 is not initialized, it will try to find and connect to the first available PPK2 device
        /// </summary>
        /// <returns></returns>
        public async Task<string> JLink_Init(bool withppk2 = true, string mcu = "nRF52840_xxAA", int speed = 4000, uint rttaddr = 0)
        {
            if (DriversServerMainDataContext.MCUMultiRTTConsole != null)
            {
                if (DriversServerMainDataContext.MCUMultiRTTConsole.IsListening)
                {
                    return "OK";
                }
            }

            // Get all available connected JLinks
            Console.WriteLine("Searching for available JLinks...");
            var connected_jlinks = JLinkDriver.GetConnectedJLinks();
            if (connected_jlinks == null)
            {
                Console.WriteLine("Cannot find any JLinks.");
                return "Cannot find any JLinks.";
            }

            var numofjlinks = connected_jlinks?.Where(j => j.SerialNumber != 0).Count();
            if (numofjlinks == 0)
            {
                Console.WriteLine("Cannot find any JLinks.");
                return "Cannot find any JLinks.";
            }

            Console.WriteLine($"{numofjlinks} JLinks found.");

            for (var i = 0; i < numofjlinks; i++)
            {
                Console.WriteLine($"SN: {connected_jlinks[i].SerialNumber}, Product: {connected_jlinks[i].acProduct}, NickName: {connected_jlinks[i].acNickName}");
            }

            // Take first available JLink
            var devsn = connected_jlinks[0].SerialNumber.ToString();

            if (withppk2)
            {
                if (DriversServerMainDataContext.PPK2_Driver == null)
                {
                    var res = DriversServerMainDataContext.FindAndConnectPPK();
                    if (res == "OK")
                    {
                        PPK2_SetVoltage(3600);
                        PPK2_TurnOn();
                    }
                    else
                    {
                        Console.WriteLine("Cannot find any PPK2 devices.");
                        return "Cannot find any PPK2 devices.";
                    }
                }
                else
                {
                    PPK2_SetVoltage(3600);
                    PPK2_TurnOn();
                }
            }

            await Task.Delay(2500);

            // Create MCUConsole instances for Config and Log RTT channels
            Console.WriteLine("JLink RTT Console is Starting :)\n\n");

            try
            {
                DriversServerMainDataContext.MCUMultiRTTConsole = new MCUMultiRTTConsole(new List<MultiRTTClientBase>()
                {
                    new MultiRTTClientBase(){ Channel = 0, DriverType= RTTDriverType.JLinkRTT, Name = "ConfigConsole" },
                    new MultiRTTClientBase(){ Channel = 1, DriverType= RTTDriverType.JLinkRTT, Name = "LogConsole" }
                }, mcu, speed, "mcumulticonsole", devsn, rttaddr);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("JLinkDriver>> Target power supply is not present."))
                {
                    Console.WriteLine($"Error while starting listening task: {ex.Message}");
                    return "Target Chip has no power.";
                }
                Console.WriteLine($"Error while creating MCUConsole: {ex.Message}");
                return "Error while creating MCUConsole.";
            }

            // Share info about jlink is connected and websocket should subscribe to the new messages
            DriversServerMainDataContext.OnJLinkConnectedEvent();

            try
            {
                DriversServerMainDataContext.cts = new CancellationTokenSource();
                Task listeningTask = DriversServerMainDataContext.MCUMultiRTTConsole.StartListening(DriversServerMainDataContext.cts.Token);

                DriversServerMainDataContext.IsConsoleListening = true;

                //await Task.WhenAny(new Task[] { listeningTask });
                DriversServerMainDataContext.JLinkTaskQueue.Enqueue(listeningTask);
                await Task.Delay(3500);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while starting listening task: {ex.Message}");
                return "Error while starting listening task.";
            }

            return "OK";
        }

        /// <summary>
        /// Stop JLink RTT Console
        /// </summary>
        /// <returns></returns>
        public string JLink_Stop()
        {
            if (DriversServerMainDataContext.MCUMultiRTTConsole == null)
                return "JLink RTT Console not initialized";

            DriversServerMainDataContext.cts?.Cancel();
            DriversServerMainDataContext.MCUMultiRTTConsole.Dispose();
            DriversServerMainDataContext.MCUMultiRTTConsole = null;

            DriversServerMainDataContext.IsConsoleListening = false;
            return "OK";
        }

        /// <summary>
        /// Send command to JLink RTT Console by channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public string JLink_SendCommand(int channel, string message)
        {
            if (DriversServerMainDataContext.MCUMultiRTTConsole == null)
                return "JLink RTT Console not initialized";
            if (!DriversServerMainDataContext.IsConsoleListening)
                return "JLink RTT Console is not listening";
            DriversServerMainDataContext.MCUMultiRTTConsole.SendCommand(channel, message);
            return "OK";
        }

        /// <summary>
        /// Send command to JLink RTT Console by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public string JLink_SendCommandByName(string name, string message)
        {
            if (DriversServerMainDataContext.MCUMultiRTTConsole == null)
                return "JLink RTT Console not initialized";
            if (!DriversServerMainDataContext.IsConsoleListening)
                return "JLink RTT Console is not listening";
            DriversServerMainDataContext.MCUMultiRTTConsole.SendCommand(name, message);
            return "OK";
        }

        public async Task<string> JLink_LoadAllCommandsFromHelp(int channel = 0, string parent = "")
        {
            if (DriversServerMainDataContext.MCUMultiRTTConsole == null)
                return "JLink RTT Console not initialized";
            if (!DriversServerMainDataContext.IsConsoleListening)
                return "JLink RTT Console is not listening";
            var res = await DriversServerMainDataContext.MCUMultiRTTConsole.LoadCommandsFromDeviceHelp(parent, 0);
            if (res != null)
            {
                if (res.Count > 0)
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(res);
                    return json;
                }
            }
            return "OK";
        }

        public async Task<string> JLink_LoadFirmware(string hash = "", string filename = "")
        {
            if (string.IsNullOrEmpty(hash) && string.IsNullOrEmpty(filename))
            {
                return "Firmware hash and filename are empty. Fill at least one";
            }

            if (!string.IsNullOrEmpty(hash) && string.IsNullOrEmpty(filename))
            {
                try
                {
                    //var url = $"https://firmware.hardwario.com/chester/{hash}/hex";
                    filename = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\{hash}.hex";
                    await HioFirmwareDownloader.DownloadFirmwareByHashAsync(hash, filename);
                }
                catch (Exception ex)
                {
                    return $"Error while downloading firmware: {ex.Message}";
                }
            }
            if (DriversServerMainDataContext.MCUMultiRTTConsole != null && DriversServerMainDataContext.MCUMultiRTTConsole.IsListening)
            {
                DriversServerMainDataContext.cts.Cancel();
                await Task.Delay(100);
                DriversServerMainDataContext.MCUMultiRTTConsole.ReconnectJLink();
                //MCUConsole.CloseAll();
                //IsConsoleListening = false;
                //OnIsJLinkConnected?.Invoke(this, false);
                await Task.Delay(150);

                try
                {
                    var res = DriversServerMainDataContext.MCUMultiRTTConsole.LoadFirmware("ConfigConsole", filename);
                    if (res)
                    {
                        Console.WriteLine("Waiting 10 seconds after reboot of MCU");
                        await Task.Delay(10000);

                        DriversServerMainDataContext.MCUMultiRTTConsole.ReconnectJLink();
                        DriversServerMainDataContext.cts = new CancellationTokenSource();
                        
                        try
                        {
                            DriversServerMainDataContext.cts = new CancellationTokenSource();
                            Task listeningTask = DriversServerMainDataContext.MCUMultiRTTConsole.StartListening(DriversServerMainDataContext.cts.Token);

                            DriversServerMainDataContext.IsConsoleListening = true;

                            //await Task.WhenAny(new Task[] { listeningTask });
                            DriversServerMainDataContext.JLinkTaskQueue.Enqueue(listeningTask);
                            await Task.Delay(3500);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error while starting listening task: {ex.Message}");
                            return "Error while starting listening task.";
                        }
                        return "OK";
                    }
                    else
                    {
                        return "Error while loading firmware.";
                    }
                }
                catch (Exception ex)
                {
                     return "Error while loading firmware.";
                }
            }
            else
            {
                return "Console is not listening. Cannot load firmware.";
            }
        }

    }
}

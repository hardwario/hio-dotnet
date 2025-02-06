using EmbedIO.Routing;
using EmbedIO;
using EmbedIO.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using hio_dotnet.HWDrivers.PPK2;
using hio_dotnet.HWDrivers.Enums;
using hio_dotnet.HWDrivers.JLink;
using hio_dotnet.HWDrivers.MCU;

namespace hio_dotnet.HWDrivers.Server
{
    public class GetPortsResponse
    {
        public string PortName { get; set; }
        public string SerialNumber { get; set; }
    }
    public class DriversApiControler : WebApiController
    {
        // GET: /api/ppk2/getportsnames
        [Route(HttpVerbs.Get, "/ppk2/getportsnames")]
        public List<GetPortsResponse> PPK2_GetPortsNames()
        {
            var res = new List<GetPortsResponse>();
            var r = PPK2_DeviceManager.ListAvailablePPK2Devices();
            foreach(var item in r)
            {
                res.Add(new GetPortsResponse() { PortName = item.PortName, SerialNumber = item.SerialNumber });
            }

            return res;
        }

        // GET: /api/ppk2/init
        [Route(HttpVerbs.Get, "/ppk2/init/{portname}")]
        public string PPK2_Init(string portname)
        {
            DriversServerMainDataContext.PPK2_Driver = new PPK2_Driver(portname);

            return "OK";
        }

        // GET: /api/ppk2/setvoltage/{voltage}
        [Route(HttpVerbs.Get, "/ppk2/setvoltage/{voltage}")]
        public string PPK2_SetVoltage(int voltage)
        {
            if (DriversServerMainDataContext.PPK2_Driver == null)
                return "PPK2 driver not initialized";

            DriversServerMainDataContext.PPK2_Driver.SetSourceVoltage(voltage);

            return "OK";
        }

        // GET: /api/ppk2/turnon
        [Route(HttpVerbs.Get, "/ppk2/turnon")]
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

        // GET: /api/ppk2/turnoff
        [Route(HttpVerbs.Get, "/ppk2/turnoff")]
        public string PPK2_TurnOff()
        {
            if (DriversServerMainDataContext.PPK2_Driver == null)
                return "PPK2 driver not initialized";

            Console.WriteLine("Turning off DUT power...");
            DriversServerMainDataContext.PPK2_Driver.ToggleDUTPower(PPK2_OutputState.OFF);
            DriversServerMainDataContext.IsDeviceOn = false;

            return "OK";
        }

        // GET: /api/ppk2/devicestatus
        [Route(HttpVerbs.Get, "/ppk2/devicestatus")]
        public bool PPK2_DeviceStatus()
        {
            return DriversServerMainDataContext.IsDeviceOn;
        }

        // GET: /api/ppk2/devicevoltage
        [Route(HttpVerbs.Get, "/ppk2/devicevoltage")]
        public int PPK2_DeviceVoltage()
        {
            return DriversServerMainDataContext.DeviceVoltage;
        }

        // GET: /api/jlink/init
        [Route(HttpVerbs.Get, "/jlink/init")]
        public async Task<string> JLink_Init()
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

            await Task.Delay(2500);

            // Create MCUConsole instances for Config and Log RTT channels
            Console.WriteLine("JLink RTT Console is Starting :)\n\n");

            try
            {
                DriversServerMainDataContext.MCUMultiRTTConsole = new MCUMultiRTTConsole(new List<MultiRTTClientBase>()
                {
                    new MultiRTTClientBase(){ Channel = 0, DriverType= RTTDriverType.JLinkRTT, Name = "ConfigConsole" },
                    new MultiRTTClientBase(){ Channel = 1, DriverType= RTTDriverType.JLinkRTT, Name = "LogConsole" }
                }, "nRF52840_xxAA", 4000, "mcumulticonsole", devsn);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while creating MCUConsole: {ex.Message}");
                return "Error while creating MCUConsole.";
            }

            // Share info about jlink is connected and websocket should subscribe to the new messages
            DriversServerMainDataContext.OnJLinkConnectedEvent();

            DriversServerMainDataContext.cts = new CancellationTokenSource();
            Task listeningTask = DriversServerMainDataContext.MCUMultiRTTConsole.StartListening(DriversServerMainDataContext.cts.Token);

            DriversServerMainDataContext.IsConsoleListening = true;

            //await Task.WhenAny(new Task[] { listeningTask });
            DriversServerMainDataContext.JLinkTaskQueue.Enqueue(listeningTask);
            
            return "OK";
        }

        // GET: /api/jlink/stop

        [Route(HttpVerbs.Get, "/jlink/stop")]
        public string JLink_Stop()
        {
            if (DriversServerMainDataContext.MCUMultiRTTConsole == null)
                return "JLink RTT Console not initialized";

            DriversServerMainDataContext.cts?.Cancel();

            DriversServerMainDataContext.IsConsoleListening = false;
            return "OK";
        }

        // GET: /api/jlink/sendcommand/{channel}/{message}
        [Route(HttpVerbs.Get, "/jlink/sendcommandbychannel/{channel}/{message}")]
        public string JLink_SendCommand(int channel, string message)
        {
            if (DriversServerMainDataContext.MCUMultiRTTConsole == null)
                return "JLink RTT Console not initialized";
            if (!DriversServerMainDataContext.IsConsoleListening)
                return "JLink RTT Console is not listening";
            DriversServerMainDataContext.MCUMultiRTTConsole.SendCommand(channel, message);
            return "OK";
        }

        // GET: /api/jlink/sendcommand/{name}/{message}
        [Route(HttpVerbs.Get, "/jlink/sendcommandbyname/{name}/{message}")]
        public string JLink_SendCommandByName(string name, string message)
        {
            if (DriversServerMainDataContext.MCUMultiRTTConsole == null)
                return "JLink RTT Console not initialized";
            if (!DriversServerMainDataContext.IsConsoleListening)
                return "JLink RTT Console is not listening";
            DriversServerMainDataContext.MCUMultiRTTConsole.SendCommand(name, message);
            return "OK";
        }

        // POST: /api/upload
        [Route(HttpVerbs.Post, "/upload")]
        public async Task<string> UploadFile()
        {
            using var stream = HttpContext.OpenRequestStream();
            var parser = new MimeParser(stream, MimeFormat.Entity);
            var entity = parser.ParseEntity();

            if (entity is MimePart part && part.Content != null)
            {
                var fileName = part.FileName ?? "uploaded_file";
                var savePath = Path.Combine("UploadedFiles", fileName);
                Directory.CreateDirectory("UploadedFiles");

                using var fileStream = File.Create(savePath);
                await part.Content.DecodeToAsync(fileStream);

                return $"File uploaded to {savePath}";
            }

            return "No file found in the request.";
        }
    }
}

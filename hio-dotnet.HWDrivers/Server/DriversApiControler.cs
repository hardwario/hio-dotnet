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
            return DriversServerMainDataContext.DriversCommonController.PPK2_GetPortNames();
        }

        // GET: /api/ppk2/init
        [Route(HttpVerbs.Get, "/ppk2/init/{portname}")]
        public string PPK2_Init(string portname)
        {
            portname = System.Net.WebUtility.UrlDecode(portname);
            return DriversServerMainDataContext.DriversCommonController.PPK2_Init(portname);
        }

        // GET: /api/ppk2/setvoltage/{voltage}
        [Route(HttpVerbs.Get, "/ppk2/setvoltage/{voltage}")]
        public string PPK2_SetVoltage(int voltage)
        {
            return DriversServerMainDataContext.DriversCommonController.PPK2_SetVoltage(voltage);
        }

        // GET: /api/ppk2/turnon
        [Route(HttpVerbs.Get, "/ppk2/turnon")]
        public string PPK2_TurnOn()
        {
            return DriversServerMainDataContext.DriversCommonController.PPK2_TurnOn();
        }

        // GET: /api/ppk2/turnoff
        [Route(HttpVerbs.Get, "/ppk2/turnoff")]
        public string PPK2_TurnOff()
        {
            return DriversServerMainDataContext.DriversCommonController.PPK2_TurnOff();
        }

        // GET: /api/ppk2/devicestatus
        [Route(HttpVerbs.Get, "/ppk2/devicestatus")]
        public bool PPK2_DeviceStatus()
        {
            return DriversServerMainDataContext.DriversCommonController.PPK2_DeviceStatus();
        }

        // GET: /api/ppk2/devicevoltage
        [Route(HttpVerbs.Get, "/ppk2/devicevoltage")]
        public int PPK2_DeviceVoltage()
        {
            return DriversServerMainDataContext.DriversCommonController.PPK2_DeviceVoltage();
        }

        // GET: /api/jlink/init
        [Route(HttpVerbs.Get, "/jlink/init/{withppk2}/{mcu}/{speed}/{rttaddr}")]
        public async Task<string> JLink_Init(bool withppk2 = true, string mcu = "nRF52840_xxAA", int speed = 4000, uint rttaddr = 0)
        {
            return await DriversServerMainDataContext.DriversCommonController.JLink_Init(withppk2, mcu, speed, rttaddr);
        }

        // GET: /api/jlink/stop

        [Route(HttpVerbs.Get, "/jlink/stop")]
        public string JLink_Stop()
        {
            return DriversServerMainDataContext.DriversCommonController.JLink_Stop();
        }

        // GET: /api/jlink/sendcommand/{channel}/{message}
        [Route(HttpVerbs.Get, "/jlink/sendcommandbychannel/{channel}/{message}")]
        public string JLink_SendCommand(int channel, string message)
        {
            message = System.Net.WebUtility.UrlDecode(message);
            return DriversServerMainDataContext.DriversCommonController.JLink_SendCommand(channel, message);
        }

        // GET: /api/jlink/sendcommand/{name}/{message}
        [Route(HttpVerbs.Get, "/jlink/sendcommandbyname/{name}/{message}")]
        public string JLink_SendCommandByName(string name, string message)
        {
            message = System.Net.WebUtility.UrlDecode(message);
            return DriversServerMainDataContext.DriversCommonController.JLink_SendCommandByName(name, message);
        }

        // GET: /api/jlink/loadcommandsfromhelp/{channel}/{parent}
        [Route(HttpVerbs.Get, "/jlink/loadcommandsfromhelp/{channel}/{parent}")]
        public async Task<string> JLink_LoadAllCommandsFromHelp(int channel, string parent)
        {
            parent = System.Net.WebUtility.UrlDecode(parent);
            return await DriversServerMainDataContext.DriversCommonController.JLink_LoadAllCommandsFromHelp(channel, parent);
        }

        // GET: /api/jlink/uploadfirmwarebyhash/{hash}
        [Route(HttpVerbs.Get, "/jlink/uploadfirmwarebyhash/{hash}")]
        public async Task<string> JLink_UploadFirmwareByHash(string hash)
        {
            return await DriversServerMainDataContext.DriversCommonController.JLink_LoadFirmware(hash, "");
        }

        // GET: /api/jlink/uploadfirmwarebyfilename/{filename}
        [Route(HttpVerbs.Get, "/jlink/uploadfirmwarebyfilename/{filename}")]
        public async Task<string> JLink_UploadFirmwareByFilename(string filename)
        {
            filename = System.Net.WebUtility.UrlDecode(filename);
            return await DriversServerMainDataContext.DriversCommonController.JLink_LoadFirmware("",filename);
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

using hio_dotnet.HWDrivers.MCU;
using hio_dotnet.HWDrivers.PPK2;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.Server
{
    public static class DriversServerMainDataContext
    {
        public static PPK2_Driver? PPK2_Driver { get; set; }
        public static MCUMultiRTTConsole? MCUMultiRTTConsole { get; set; }

        /// <summary>
        /// Shared functions of controller for API and WebSocket controllers.
        /// </summary>
        public static DriversCommonController DriversCommonController { get; set; } = new DriversCommonController();

        /// <summary>
        /// When the JLink is connected over the api, this event is fired.
        /// </summary>
        public static event EventHandler? OnJLinkConnected;

        /// <summary>
        /// When the JLink is started over the api, the task is added to this queue and must be processed by the main thread.
        /// </summary>
        public static ConcurrentQueue<Task> JLinkTaskQueue { get; set; } = new ConcurrentQueue<Task>();

        public static bool IsConsoleListening { get; set; } = false;
        public static bool IsDeviceOn { get; set; } = false;
        public static int DeviceVoltage { get; set; } = 0;
        public static string ServerBaseUrl { get; set; } = "http://localhost:8042";

        public static CancellationTokenSource? cts;

        public static void OnJLinkConnectedEvent()
        {
            OnJLinkConnected?.Invoke(null, EventArgs.Empty);
        }

        public static string FindAndConnectPPK()
        {
            var devices = PPK2_DeviceManager.ListAvailablePPK2Devices();

            if (devices.Count == 0)
            {
                Console.WriteLine("No PPK2 devices found. Exiting program...");
                return "No PPK2 devices found.";
            }

            var selectedDevice = devices[0];
            Console.WriteLine($"\nUsing PPK2 device on COM Port: {selectedDevice.PortName} with Serial Number: {selectedDevice.SerialNumber}");

            PPK2_Driver = new PPK2_Driver(selectedDevice.PortName);

            return "OK";
        }
    }
}

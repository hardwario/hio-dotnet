using hio_dotnet.HWDrivers.Enums;
using hio_dotnet.HWDrivers.JLink;
using hio_dotnet.HWDrivers.MCU;
using hio_dotnet.HWDrivers.PPK2;
using System.Diagnostics;

var JLINK_TEST = false;
var PPK2_TEST = true;

#if WINDOWS
    Console.WriteLine("Running on Windows");
#elif LINUX
    Console.WriteLine("Running on Linux");
#elif OSX
    Console.WriteLine("Running on macOS");
#else
Console.WriteLine("Unknown platform");
#endif

#region JLinkExample
if (JLINK_TEST)
{

    // Get all available connected JLinks
    Console.WriteLine("Searching for available JLinks...");
    var connected_jlinks = JLinkDriver.GetConnectedJLinks();
    if (connected_jlinks != null)
    {
        var numofjlinks = connected_jlinks?.Where(j => j.SerialNumber != 0).Count();
        if (numofjlinks == 0)
        {
            Console.WriteLine("Cannot find any JLinks.");
            return;
        }

        Console.WriteLine($"{numofjlinks} JLinks found.");

        for (var i = 0; i < numofjlinks; i++)
        {
            Console.WriteLine($"SN: {connected_jlinks[i].SerialNumber}, Product: {connected_jlinks[i].acProduct}, NickName: {connected_jlinks[i].acNickName}");
        }

        // Take first available JLink
        var devsn = connected_jlinks[0].SerialNumber.ToString();

        // Create MCUConsole instances for Config and Log RTT channels
        Console.WriteLine("JLink RTT Console is Starting :)\n\n");
        var consoleConfig = new MCUConsole(RTTDriverType.JLinkRTT, "nRF52840_xxAA", 4000, 0, "ConfigConsole", devsn);
        var consoleLog = new MCUConsole(RTTDriverType.JLinkRTT, "nRF52840_xxAA", 4000, 1, "LogConsole", devsn);

        // Subscribe to NewRTTMessageLineReceived event to get RTT messages and set output to console
        consoleConfig.NewRTTMessageLineReceived += (sender, line) =>
        {
            Console.WriteLine($"MCU Config Message: {line}");
        };

        consoleLog.NewRTTMessageLineReceived += (sender, line) =>
        {
            Console.WriteLine($"MCU Log Message: {line}");
        };

        var cts = new CancellationTokenSource();
        Task listeningTaskLogs = consoleLog.StartListening(cts.Token);
        Task listeningTaskConsole = consoleConfig.StartListening(cts.Token);

        // Try to send some command to the MCU
        var command = "config show";
        Console.WriteLine("Sending Command: " + command);
        await consoleConfig.SendCommand(command);

        await Task.Delay(1500);

        // Try to load new firmware to the MCU

        // chester current firmware
        //string url_push = "https://firmware.hardwario.com/chester/a2f47dd13c1f4a94ae68af09aa54e089/hex";
        string url_current = "https://firmware.hardwario.com/chester/52177a80039543d38725d4d9f57590ea/hex";

        string savePath = "firmware.hex";

        // delete file if already exists
        if (File.Exists(savePath))
            File.Delete(savePath);

        // Downlaod firmware
        await HioFirmwareDownloader.DownloadFirmwareAsync(url_current, savePath);
        // Load firmware
        await consoleConfig.LoadFirmware(savePath);

        // wait some time after reboot of MCU
        Console.WriteLine("Waiting 10 seconds after reboot of MCU");
        await Task.Delay(10000);

        // Reconnect RTT after reset of MCU
        consoleConfig.ReconnectJLink();
        consoleLog.ReconnectJLink();

        Console.WriteLine("Sending Command: " + command);
        await consoleConfig.SendCommand(command);

        // Quit when key is pressed
        Console.WriteLine("\nPress any key to stop listening...\n");
        Console.ReadKey();
        cts.Cancel();

        await Task.WhenAny(new Task[] { listeningTaskConsole, listeningTaskLogs });

        consoleConfig.Dispose();
        consoleLog.Dispose();

        Console.WriteLine("Program ends. Goodbye");
    }

}
#endregion

#region PPK2Example
if (PPK2_TEST)
{
    // Setup logging
    var logger = new TraceSource("PPK2DeviceManager", SourceLevels.All);
    logger.Listeners.Add(new ConsoleTraceListener()); // Log to console
    logger.Listeners.Add(new TextWriterTraceListener("PPK2DeviceManager.log")); // Log to file

    logger.TraceInformation("Starting PPK2 Device Manager");

    try
    {
        // Find all available PPK2 devices
        var devices = PPK2_DeviceManager.ListAvailablePPK2Devices();

        if (devices.Count == 0)
        {
            logger.TraceEvent(TraceEventType.Warning, 0, "No PPK2 devices found.");
            Console.WriteLine("No PPK2 devices found. Exiting program...");
            return;
        }

        // Display all found devices
        Console.WriteLine("Available PPK2 devices:");
        foreach (var device in devices)
        {
            Console.WriteLine($"COM Port: {device.PortName}, Serial Number: {device.SerialNumber}");
        }

        // Select the first available PPK2 device
        var selectedDevice = devices[0];
        Console.WriteLine($"\nUsing PPK2 device on COM Port: {selectedDevice.PortName} with Serial Number: {selectedDevice.SerialNumber}");
        logger.TraceInformation($"Using PPK2 device on COM Port: {selectedDevice.PortName} with Serial Number: {selectedDevice.SerialNumber}");

        // Initialize the PPK2 Driver with the selected COM port
        using (var ppk2 = new PPK2_Driver(selectedDevice.PortName))
        {
            // Set a source voltage (e.g., 3300 mV)
            int voltage = 3300;
            Console.WriteLine($"Setting source voltage to {voltage} mV...");
            logger.TraceInformation($"Setting source voltage to {voltage} mV...");
            ppk2.SetSourceVoltage(voltage);

            // Turn on the DUT power
            Console.WriteLine("Turning on DUT power...");
            logger.TraceInformation("Turning on DUT power...");
            ppk2.ToggleDUTPower(PPK2_OutputState.ON);

            // Use the device in source meter mode
            Console.WriteLine("Switching to source meter mode...");
            logger.TraceInformation("Switching to source meter mode...");
            ppk2.UseSourceMeter();

            // Start measuring
            Console.WriteLine("Starting measurement...");
            logger.TraceInformation("Starting measurement...");
            ppk2.StartMeasuring();

            // Collect data for X seconds
            var ontime = 10;
            Console.WriteLine($"Collecting data for {ontime} seconds...");
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds < ontime)
            {
                byte[] data = ppk2.GetData();

                if (data.Length > 0)
                {
                    string dataStr = BitConverter.ToString(data);
                    Console.WriteLine($"Received Data: {dataStr}");
                    logger.TraceInformation($"Received Data: {dataStr}");
                }

                await Task.Delay(500); // Adjust the sleep duration as needed
            }

            // Stop measuring
            Console.WriteLine("Stopping measurement...");
            logger.TraceInformation("Stopping measurement...");
            ppk2.StopMeasuring();

            // Turn off the DUT power
            Console.WriteLine("Turning off DUT power...");
            logger.TraceInformation("Turning off DUT power...");
            ppk2.ToggleDUTPower(PPK2_OutputState.OFF);

            Console.WriteLine("Measurement complete. Exiting program...");
            logger.TraceInformation("Measurement complete. Exiting program...");
        }
    }
    catch (Exception ex)
    {
        logger.TraceEvent(TraceEventType.Error, 0, $"An error occurred: {ex.Message}");
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
    finally
    {
        // Flush and close the logger
        foreach (TraceListener listener in logger.Listeners)
        {
            listener.Flush();
            listener.Close();
        }
    }
}
#endregion
using hio_dotnet.APIs.Chirpstack;
using hio_dotnet.APIs.ThingsBoard;
using hio_dotnet.HWDrivers.Enums;
using hio_dotnet.HWDrivers.JLink;
using hio_dotnet.HWDrivers.MCU;
using hio_dotnet.HWDrivers.PPK2;
using System.Diagnostics;

var JLINK_TEST = false;
var PPK2_TEST = false;
var CHIRPSTACK_TEST = false;
var THINGSBOARD_TEST = false;

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

#region CHIRPSTACKExample
if (CHIRPSTACK_TEST)
{
    // This must point to the API interface.
    var server = "http://192.168.1.132";

    // Example deveui and tenantId (retrieved using the web-interface)
    // !!! You must set your own deveui and tenantId!!!
    var devEui = "3cb77038dbda3643";
    var tenantId = "52f14cd4-c6f1-4fbd-8f87-4025e1d49242";

    // The API token (retrieved using the web-interface).
    // !!! You must set your own api key !!!
    //var apiToken = "YOUR_CHIRPSTACK_APIKEY";
    var apiToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJjaGlycHN0YWNrIiwiaXNzIjoiY2hpcnBzdGFjayIsInN1YiI6ImE1OWJmMzMyLTVkYzYtNGRkMC04ZGMyLTVmOTE5Yjc3YjcwZCIsInR5cCI6ImtleSJ9.OipZYaLOW8FoIpAAYz-zI4JHPxl5I04FOvxsFZXy8c4";

    var cs = new ChirpStackDriver(apiToken, server, 8080, false);

    var resp = await cs.ListApplications(tenantId);

    // Print the downlink id.
    Console.WriteLine("Total count of applications: " + resp.TotalCount.ToString());
    Console.WriteLine("");
    foreach (var app in resp.Result)
    {
        Console.WriteLine($"AppName: {app.Name}, AppId: {app.Id}");

        Console.WriteLine("Listing devices...");
        var devs = await cs.ListDevicesInApplication(app.Id);
        foreach (var dev in devs.Result)
        {
            Console.WriteLine($"DeviceName: {dev.Name}, DeviceEui: {dev.DevEui}");
            var devdetails = cs.GetDeviceDetails(dev.DevEui);

            if (devdetails != null && devdetails.Result != null)
            {
                Console.WriteLine($"Device Last Seen: {devdetails.Result.LastSeenAt}");
                Console.WriteLine($"Device JoinEUI: {devdetails.Result.Device.JoinEui}");
            }
            try
            {
                var devkeys = cs.GetDeviceKeys(dev.DevEui);
                if (devkeys != null && devkeys.Result != null)
                {
                    Console.WriteLine($"Device AppKey: {devkeys.Result.DeviceKeys.AppKey}");
                    Console.WriteLine($"Device NwkKey: {devkeys.Result.DeviceKeys.NwkKey}");

                }
            }
            catch { }

            try
            {
                var devactivation = cs.GetDeviceActivation(dev.DevEui);
                if (devactivation != null && devactivation.Result != null)
                {
                    Console.WriteLine($"Device Address: {devactivation.Result.DeviceActivation.DevAddr}");
                    Console.WriteLine($"Device AppsKey: {devactivation.Result.DeviceActivation.AppSKey}");
                    Console.WriteLine($"Device Enc NwkSKey: {devactivation.Result.DeviceActivation.NwkSEncKey}");
                    Console.WriteLine($"Device FNwkSIntKey: {devactivation.Result.DeviceActivation.FNwkSIntKey}");
                    Console.WriteLine($"Device SNwkSIntKey: {devactivation.Result.DeviceActivation.SNwkSIntKey}");

                }
            }
            catch { }
            Console.WriteLine("____________________");
        }
        Console.WriteLine("----------------------------------------------------------");
        Console.WriteLine("");
    }

    var deviceProfiles = await cs.ListDeviceProfiles(tenantId);
    var devprofId = "";
    if (deviceProfiles != null && deviceProfiles.Result != null)
    {
        Console.WriteLine("Listing Device Profiles...");
        foreach (var dp in deviceProfiles.Result)
        {
            Console.WriteLine($"Device Profile Name: {dp.Name}, Device Profile Id: {dp.Id}");
            var profile = cs.GetDeviceProfile(dp.Id);
            if (profile != null && profile.Result != null)
            {
                if (profile.Result.DeviceProfile.Name == "devkitProfile")
                {
                    devprofId = profile.Result.DeviceProfile.Id;
                }

                Console.WriteLine($"\tDevice Profile Name: {profile.Result.DeviceProfile.Name}");
                Console.WriteLine($"\tDevice Profile Id: {profile.Result.DeviceProfile.Id}");
                Console.WriteLine($"\tDevice Profile MacVersion: {profile.Result.DeviceProfile.MacVersion}");
                Console.WriteLine($"\tDevice Profile Region: {profile.Result.DeviceProfile.Region}");
                Console.WriteLine($"\tDevice Profile Supports OTAA: {profile.Result.DeviceProfile.SupportsOtaa}");
            }
            Console.WriteLine("____________________");
        }
        Console.WriteLine("----------------------------------------------------------");
        Console.WriteLine("");
    }

    Console.WriteLine("Creating Application...");
    var addapp = await cs.CreateApplication("testapp", tenantId, "just testing app");
    Console.WriteLine($"Application created: {addapp.Id}");

    if (addapp != null && !string.IsNullOrEmpty(addapp.Id))
    {
        Console.WriteLine("Creating Device...");
        var adddev = await cs.CreateDevice(ChirpStackDriver.GetRandomDevEUI(), "testdevice", addapp.Id, devprofId, ChirpStackDriver.GetRandomJoinEUI());
        Console.WriteLine($"Device created.");
        var newdeveui = "";

        Console.WriteLine($"Getting all devices of new application: {addapp.Id}");
        var appdevices = await cs.ListDevicesInApplication(addapp.Id);
        foreach (var dev in appdevices.Result)
        {
            Console.WriteLine($"DeviceName: {dev.Name}, DeviceEui: {dev.DevEui}");
            var devdetails = cs.GetDeviceDetails(dev.DevEui);
            newdeveui = dev.DevEui;

            if (devdetails != null && devdetails.Result != null)
            {
                Console.WriteLine($"Device Last Seen: {devdetails.Result.LastSeenAt}");
                Console.WriteLine($"Device JoinEUI: {devdetails.Result.Device.JoinEui}");
            }
        }

        // set this to false if you want to see result of created app and device in Chirpstack web ui
        // if this is set to true the device and app will be deleted to test the delete functions
        // You can place breakpoint to the next line to see the results in the web ui and then continue to delete items automatically
        var deletedevicetest = true;

        if (deletedevicetest)
        {
            Console.WriteLine("Deleting Device...");
            var deldev = await cs.DeleteDevice(newdeveui);
            Console.WriteLine($"Device deleted.");

            Console.WriteLine("Deleting Application...");
            var delapp = await cs.DeleteApplication(addapp.Id);
            Console.WriteLine($"Application deleted.");
        }
    }

    Console.WriteLine("\n\n");
    Console.WriteLine("Press key to exit...");
    Console.ReadKey();
}
#endregion

#region THINGSBOARDExample

if (THINGSBOARD_TEST)
{
    // You need to have ThingsBoard running on localhost to test this example

    // Default localhost ThingsBoard API URL
    var baseUrl = "http://localhost";

    // Device ID. Use your device id
    var deviceId = "9d6f6e10-5a6f-11ef-8883-e9c7bb918e65";

    // Specify the keys you want to obtain. If multiple keys are required use ',' as separator
    // be sure that device telemetry contains those keys otherwise put there some existing keys
    var keys = "therm_temperature,hygro_humidity";

    var username = "tenant@thingsboard.org";
    var password = "tenant";

    Console.WriteLine("Initializing driver and connection...");
    var thingsBoardDriver = new ThingsBoardDriver(baseUrl, username, password, 8080);
    Console.WriteLine("Connection initialized.");

    Console.WriteLine("Getting actual newest data from API...");
    var data = await thingsBoardDriver.GetTelemetryDataAsync(deviceId, keys);
    Console.WriteLine("Parsing the actual data...");
    var parsedData = await thingsBoardDriver.ParseActualTelemetryDataAsync(data);
    Console.WriteLine("Data parsed.\n");
    foreach (var item in parsedData)
    {
        Console.WriteLine($"\tTimestamp: {item.Value.Timestamp}");
        Console.WriteLine($"\t{item.Key}: {item.Value.Value}");
    }

    Console.WriteLine("\nGetting historic data...");
    var historicalData = await thingsBoardDriver.GetTelemetryDataForTimeRangeAsync(deviceId, keys, DateTime.Now.AddDays(-1), DateTime.Now);
    Console.WriteLine("Historic data obtained from api.");
    Console.WriteLine("Parsing historic data...");
    var parsedHistoricalData = await thingsBoardDriver.ParseHistoricTelemetryDataAsync(historicalData);
    Console.WriteLine("Historic data parsed.");
    Console.WriteLine("Historic data: \n\n");

    foreach (var item in parsedHistoricalData)
    {
        Console.WriteLine($"{item.Key}\n");

        var unit = "°C";

        if (item.Key.Contains("temp"))
            unit = "°C";
        else if (item.Key.Contains("hygro"))
            unit = "%";
        else if (item.Key.Contains("weight"))
            unit = "kg";

        foreach (var dataItem in item.Value)
        {
            Console.WriteLine($"\t{dataItem.Timestamp}: {dataItem.Value} {unit}");
        }
    }

    Console.WriteLine("\nPress key to quit...");
    Console.ReadKey();
}


#endregion
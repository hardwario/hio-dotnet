using hio_dotnet.APIs.Chirpstack;
using hio_dotnet.APIs.HioCloudv2;
using hio_dotnet.APIs.HioCloudv2.Models;
using hio_dotnet.APIs.ThingsBoard;
using hio_dotnet.APIs.ThingsBoard.Models.Dashboards;
using hio_dotnet.APIs.Wmbusmeters;
using hio_dotnet.Common.Models;
using hio_dotnet.Common.Models.CatalogApps;
using hio_dotnet.Common.Models.CatalogApps.Clime;
using hio_dotnet.Common.Models.CatalogApps.Counter;
using hio_dotnet.Common.Models.CatalogApps.Dust;
using hio_dotnet.Common.Models.CatalogApps.Push;
using hio_dotnet.Common.Models.CatalogApps.Radon;
using hio_dotnet.Common.Models.DataSimulation;
using hio_dotnet.HWDrivers.Enums;
using hio_dotnet.HWDrivers.JLink;
using hio_dotnet.HWDrivers.MCU;
using hio_dotnet.HWDrivers.PPK2;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

var JLINK_TEST = false;
var JLINK_COMBINED_CONSOLE_TEST = false;
var PPK2_TEST = false;
var CHIRPSTACK_TEST = false;
var THINGSBOARD_TEST = false;
var HIOCLOUDV2_TEST = false;
var HIOCLOUDV2_TEST_DOWNLINK = false;
var HIOCLOUDV2_TEST_ADD_DEVICE_WITH_CONNECTOR = false;
var HIO_WMBUSMETER_TEST = false;
var HIO_SIMULATOR_TEST = false;
var HIO_SIMULATOR_HANDLER_TEST = true;

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

#region JLinkCombinedConsoleExample
if (JLINK_COMBINED_CONSOLE_TEST)
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

    using (var ppk2 = new PPK2_Driver(selectedDevice.PortName))
    {
        // Set a source voltage (e.g., 3300 mV)
        int voltage = 3300;
        Console.WriteLine($"Setting source voltage to {voltage} mV...");
        ppk2.SetSourceVoltage(voltage);

        // Turn on the DUT power
        Console.WriteLine("Turning on DUT power...");
        ppk2.ToggleDUTPower(PPK2_OutputState.ON);

        // Take first available JLink
        var devsn = connected_jlinks[0].SerialNumber.ToString();

        // Create MCUConsole instances for Config and Log RTT channels
        Console.WriteLine("JLink RTT Console is Starting :)\n\n");

        var multiconsole = new MCUMultiRTTConsole(new List<MultiRTTClientBase>()
        {
            new MultiRTTClientBase(){ Channel = 0, DriverType= RTTDriverType.JLinkRTT, Name = "ConfigConsole" },
            new MultiRTTClientBase(){ Channel = 1, DriverType= RTTDriverType.JLinkRTT, Name = "LogConsole" }
        }, "nRF52840_xxAA", 4000, "mcumulticonsole", devsn);

        // Subscribe to NewRTTMessageLineReceived event to get RTT messages and set output to console
        multiconsole.NewRTTMessageLineReceived += (sender, data) =>
        {
            if (data?.Item2.Channel == 0)
                Console.WriteLine($"MCU Config Message: {data.Item1}");
            else if (data?.Item2.Channel == 1)
                Console.WriteLine($"MCU Log Message: {data.Item1}");
        };

        var cts = new CancellationTokenSource();
        Task listeningTask = multiconsole.StartListening(cts.Token);

        // Try to send some command to the MCU
        var command = "config show";
        Console.WriteLine("Sending Command: " + command);
        await multiconsole.SendCommand(0, command);

        await Task.Delay(1500);

        // Try to load new firmware to the MCU

        // chester current firmware
        //string url_push = "a2f47dd13c1f4a94ae68af09aa54e089";
        string hash_current = "52177a80039543d38725d4d9f57590ea";

        string savePath = "firmware.hex";

        // delete file if already exists
        if (File.Exists(savePath))
            File.Delete(savePath);

        // get firmware info
        var fwinfo = await HioFirmwareDownloader.GetFirmwareInfoAsync(hash_current);
        if (fwinfo != null)
        {
            // print fwinfo intended JSON
            Console.WriteLine("Firmware info:");
            Console.WriteLine(JsonSerializer.Serialize(fwinfo, new JsonSerializerOptions() { WriteIndented = true }));
            Console.WriteLine("\n");
        }

        // Downlaod firmware
        await HioFirmwareDownloader.DownloadFirmwareByHashAsync(hash_current, savePath);

        // turn off the RTT listening
        cts.Cancel();

        // Load firmware
        await multiconsole.LoadFirmware("ConfigConsole", savePath);

        // wait some time after reboot of MCU
        Console.WriteLine("Waiting 10 seconds after reboot of MCU");
        await Task.Delay(10000);

        // Reconnect RTT after reset of MCU
        multiconsole.ReconnectJLink();
        cts = new CancellationTokenSource();
        listeningTask = multiconsole.StartListening(cts.Token);

        Console.WriteLine("Sending Command: " + command);
        await multiconsole.SendCommand(0, command);

        // Quit when key is pressed
        Console.WriteLine("\nPress any key to stop listening...\n");
        Console.ReadKey();
        cts.Cancel();

        await Task.WhenAny(new Task[] { listeningTask });

        multiconsole.Dispose();

        Console.WriteLine("Turning off DUT power...");
        ppk2.ToggleDUTPower(PPK2_OutputState.OFF);

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

            var measureWithLoop = false;
            var measureToFile = !measureWithLoop;

            if (measureWithLoop)
            {
                // Start measuring
                Console.WriteLine("Starting measurement...");
                logger.TraceInformation("Starting measurement...");
                ppk2.StartMeasuring();

                // wait some time for init of the device
                await Task.Delay(5000);

                // Collect data for X seconds
                var ontime = 2;
                Console.WriteLine($"Collecting data for {ontime} seconds...");
                DateTime startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalSeconds < ontime)
                {
                    byte[] data = ppk2.GetData();

                    if (data.Length > 0)
                    {
                        List<double> samples = ppk2.ProcessData(data);

                        foreach (var sample in samples)
                        {
                            Console.WriteLine($"Current: {sample} μA");
                        }
                    }

                    await Task.Delay(10);
                }
            }
            
            if (measureToFile)
            {
                var duration = 2000; // Time of measurement in miliseconds (2 seconds)
                var filename = "captureddata.csv";

                await ppk2.CaptureMeasurement(filename, duration, addTimestampPrefix: true, useDotForDecimals: false);
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
    Console.WriteLine("Starting ThingsBoard test...");

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

    Console.WriteLine("\nCreating new Customer...");
    var newCustomer = await thingsBoardDriver.CreateCustomerAsync(new hio_dotnet.APIs.ThingsBoard.Models.CreateCustomerRequest()
    {
        Title = "Test Customer",
    });

    Console.WriteLine("Adding new user to the customer...");
    var sendActivationEmail = false;
    var newUser = await thingsBoardDriver.AddUserToCustomer(new hio_dotnet.APIs.ThingsBoard.Models.User()
    {
        Authority = "CUSTOMER_USER",
        CustomerId = newCustomer.Id,
        TenantId = newCustomer.TenantId,
        Email = "testemail@hardwario.com",
        FirstName = "Test",
        LastName = "Tester"
    }, sendActivationEmail);
    
    Console.WriteLine($"New Customer created: {newCustomer.Id.Id}");

    Console.WriteLine("\nGetting device profiles...");
    var deviceProfiles = await thingsBoardDriver.GetDeviceProfilesAsync();
    Console.WriteLine("Device profiles obtained.");
    foreach (var profile in deviceProfiles)
    {
        Console.WriteLine($"Device Profile: {profile.Name}, Id: {profile.Id.Id}");
    }

    var defaultProfile = deviceProfiles.Where(p => p.Name == "default").FirstOrDefault();
    if (defaultProfile != null)
    {
        // create new device under this customer and deviceProfile
        Console.WriteLine("\nCreating new device...");
        var newDevice = await thingsBoardDriver.CreateDeviceAsync(new hio_dotnet.APIs.ThingsBoard.Models.CreateDeviceRequest()
        {
            Name = "Test Device",
            Label = "Test Device",
            CustomerId = newCustomer.Id,
            DeviceProfileId = defaultProfile.Id
        });
        if (newDevice != null)
        {
            Console.WriteLine($"Device Created. Device Id: {newDevice.Id.Id}");
            Console.WriteLine("Getting Connection Info...");
            var connectionInfo = await thingsBoardDriver.GetDeviceConnectionInfoAsync(newDevice.Id.Id.ToString());
            var connectionToken = string.Empty;
            if (connectionInfo != null)
            {
                Console.WriteLine($"\nExample Device Telemetry Request: {connectionInfo.Http.Http}\n");
                connectionToken = thingsBoardDriver.ParseConnectionToken(connectionInfo);
                Console.WriteLine($"Device Connection Token: {connectionToken}");
            }

            // place this file with demo dashboard for CHESTER Counter in the app root folder
            // otherwise this part will be skipped
            var dashboard_filename = "chester_counter (5).json";
            if (File.Exists(dashboard_filename))
            {
                Console.WriteLine("Loading test dashboard...");
                var test_dashboard = File.ReadAllText(dashboard_filename);
                var test_dasboard_serialized = JsonSerializer.Deserialize<CreateDashboardRequest>(test_dashboard);
                Console.WriteLine("test dashboard loaded. Creating dashboard...");
                test_dasboard_serialized.CustomerId = newCustomer.Id;
                test_dasboard_serialized.TenantId = newCustomer.TenantId;
                foreach (var widget in test_dasboard_serialized.Configuration.Widgets)
                {
                    foreach(var datasource in widget.Value.Config.Datasources)
                        datasource.DeviceId = newDevice.Id.Id.ToString();
                }

                var newDashboard = await thingsBoardDriver.CreateDashboardAsync(test_dasboard_serialized);
                Console.WriteLine($"Dashboard created: {newDashboard.Id.Id}");


                Console.WriteLine("\nCreating Sample Data");
                var counterdevice = new ChesterCounterCloudMessage();
                BaseSimulator.GetSimulatedData(counterdevice);
                Console.WriteLine($"Sending simulated data...");
                var res = await thingsBoardDriver.SendTelemetryData(counterdevice, connectionToken);
                Console.WriteLine("Sample data sent.");

                Console.WriteLine("\n Deleting Dashboard...");
                var delDashboard = await thingsBoardDriver.DeleteDashboardAsync(newDashboard.Id.Id.ToString());
                Console.WriteLine($"Dashboard deleted.");
            }

            Console.WriteLine("\nDeleting Test device...");
            var delDevice = await thingsBoardDriver.DeleteDeviceAsync(newDevice.Id.Id.ToString());
            Console.WriteLine($"Device deleted.");
        }
    }

    Console.WriteLine("Deleting User...");
    var deleteUser = await thingsBoardDriver.DeleteCustomerUserAsync(newUser.Id.Id.ToString());
    Console.WriteLine($"User deleted.");

    Console.WriteLine("\nDeleting Customer...");
    var delCustomer = await thingsBoardDriver.DeleteCustomerAsync(newCustomer.Id.Id.ToString());
    Console.WriteLine($"Customer deleted.");

    Console.WriteLine("\nPress key to quit...");
    Console.ReadKey();
    return;


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

#region HIOCLOUDV2Example

if(HIOCLOUDV2_TEST)
{
    var email = "YOUR EMAIL";
    var password = "YOUR PASSWORD";

    var apitoken = "YOUR API TOKEN IF YOU HAVE CREATED IT IN HIO CLOUD SPACE";

    // you can init the driver with use of email and password
    // you need to use this if you want to list all available spaces because api tokens are just for specific space
    Hiov2CloudDriver hiocloudJWT = null; 
    if (email != "YOUR EMAIL" && password != "YOUR PASSWORD")
        hiocloudJWT = new Hiov2CloudDriver(Hiov2CloudDriver.DefaultHardwarioThingsboardUrl, email, password);

    if (hiocloudJWT == null && apitoken == "YOUR API TOKEN IF YOU HAVE CREATED IT IN HIO CLOUD SPACE")
    {
        Console.WriteLine("Please set your email and password or API token to test this example.");
        throw new Exception("Please set your email and password or API token to test this example.");
    }

    // or if you have already created the API token in HIO Cloud space you can use it
    var hiocloudAPI = new Hiov2CloudDriver(Hiov2CloudDriver.DefaultHardwarioThingsboardUrl, apitoken, true);

    if (hiocloudJWT != null)
    {
        var spaces = await hiocloudJWT.GetSpaces();
        Console.WriteLine("\n");
        if (spaces != null)
        {
            Console.WriteLine("Spaces:");
            foreach (var space in spaces)
            {
                Console.WriteLine($"Space: {space.Name}, Type: {space.Type}, CreatedAt: {space.CreatedAt}");
            }
        }
        else
        {
            Console.WriteLine("No spaces found.");
        }
    }

    // fill your own space id
    var demo_space = new Guid("0189f3f4-9549-78ed-959c-cf4a0b28e28c");
    var wall_space = new Guid("0190a1c8-e61e-7216-b057-ff32a8d27756");
    var devices = await hiocloudAPI.GetAllDevicesOfSpace(wall_space);
    //var devices = await hiocloudAPI.GetAllDevicesOfSpace(demo_space, tags_ids: new string[] { "01922945-0c04-7258-ae68-5b6ef7db4421" });

    if (devices != null)
    {
        Console.WriteLine("\n");
        Console.WriteLine("Devices:");
        foreach (var d in devices)
        {
            Console.WriteLine($"Device: {d.Name}, Id: {d.Id}, SpaceId: {d.SpaceId}, LastSeen: {d.LastSeen}");
        }
    }
    else
    {
        Console.WriteLine("No devices found.");
    }

    // fill your own device id
    var specific_device = new Guid("0190a1da-1a4d-7bdb-acb9-70a6c702789f");
    var device = await hiocloudAPI.GetDevice(wall_space, specific_device);

    if (device != null)
    {
        Console.WriteLine("\n");
        Console.WriteLine("Specific Device:");
        Console.WriteLine($"Device: {device.Name}, Id: {device.Id}, SpaceId: {device.SpaceId}, LastSeen: {device.LastSeen}");
    }
    else
    {
        Console.WriteLine("No device found.");
    }

    var messages = await hiocloudAPI.GetAllDeviceMessages(wall_space, specific_device);

    if (messages != null)
    {
        Console.WriteLine("\n");
        Console.WriteLine("Messages:");
        foreach (var m in messages)
        {
            Console.WriteLine($"Message: {m.Type}, Time: {m.CreatedAt}, \n\nPayload:\n {m.Body}\n\n");
            if (m.Type == HioCloudv2MessageType.Data)
            {
                Type guessedType = ChesterCloudMessageAutoIdentifier.FindTypeByMessageStructure(m.Body);
                var parsed = System.Text.Json.JsonSerializer.Deserialize(m.Body, guessedType);
                if (parsed != null)
                {
                    dynamic deserializedObject = parsed;
                    Console.WriteLine($"Guessed Type of the data message: {deserializedObject}");

                    string jsonContent = System.Text.Json.JsonSerializer.Serialize(parsed, new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true // Print output nicely formatted
                    });

                    Console.WriteLine($"Parsed JSON Content:\n\n{jsonContent}\n\n");

                }
                // skip all other messages, take just first data message to parse and printout to console
                break;
            }
        }
    }
    else
    {
        Console.WriteLine("No messages found.");
    }

    Console.WriteLine("Press key to quit...");
    Console.ReadKey();

}

#endregion


#region HIOCLOUDV2DownlinkExample
if (HIOCLOUDV2_TEST_DOWNLINK)
{
    var apitoken = "YOUR_API_TOKEN";
    var spaceid = "018ab6bb-ae9e-73ca-8917-aaae0ab1c691";
    var deviceid = "0192579f-35a8-7b29-9fd5-cdd8ac08d13a"; // nove device
    //var deviceid = "0189df80-834e-7df4-98a0-25030f81076d"; // andon-34

    var hiocloudAPI = new Hiov2CloudDriver(Hiov2CloudDriver.DefaultHardwarioThingsboardUrl, apitoken, true);

    var data = new
    {
        button_1_state = 1,
        buzzer = 5
    };

    var res = await hiocloudAPI.AddNewDownlingMessage(new Guid(spaceid), 
                                                      new Guid(deviceid), 
                                                      data, 
                                                      HioCloudv2MessageType.Data);
}
#endregion

#region HIOCLOUDV2AddDeviceAndConnectorExample
if (HIOCLOUDV2_TEST_ADD_DEVICE_WITH_CONNECTOR)
{
    var apitoken = "HIO_CLOUD_V2_API_TOKEN";
    var spaceid = new Guid("HIO_CLOUD_V2_SPACE_ID");
    var devicetoken = "THINGSBOARD_DEVICE_CONNECTION_TOKEN";

    var hiocloudAPI = new Hiov2CloudDriver(Hiov2CloudDriver.DefaultHardwarioThingsboardUrl, apitoken, true);

    var tag = new HioCloudv2Tag()
                  .WithName("test-tag")
                  .WithColor("#0000FF");

    Console.WriteLine("Creating new tag...");
    var newTag = await hiocloudAPI.CreateTag(spaceid, tag);
    Console.WriteLine($"Tag created: {newTag.Id}");

    var device = new HioCloudv2Device()
                        .WithName("test-device")
                        .WithSpaceId(spaceid)
                        .WithSerialNumber(BaseSimulator.GenerateSerialNumberString())
                        .WithToken(HioCloudv2Device.GenerateClaimToken())
                        .WithTag(newTag);

    Console.WriteLine("Creating new device...");
    var newDevice = await hiocloudAPI.CreateDevice(spaceid, device);
    Console.WriteLine($"Device created: {newDevice.Id}");

    var connector = new HioCloudv2Connector()
                        .WithName("test-connector")
                        .WithDirection("up")
                        .WithTag(newTag)
                        .WithTrigger("data")
                        .WithTrigger("session")
                        .WithThingsBoardConnectionToken(devicetoken);

    Console.WriteLine("Creating new connector...");
    var newConnector = await hiocloudAPI.CreateConnector(spaceid, connector);
    Console.WriteLine("Connector created.");


    Console.WriteLine("Press key to quit...");
    Console.ReadKey();
}
#endregion

#region HIO_WMBUSMETERExample
if (HIO_WMBUSMETER_TEST)
{
    var driver = new WMBusAPIDriver();

    var telegram = "3e4401067075340605077aa90030853b0d89f380b805889c74e194c350b41ed1c59b4ec5565d0aa77fec0d5c5a51be5f8e238df7176a1bca55ca0b8bed8f5e";
    var pass = "000000000000000000000000000000";

    Console.WriteLine("Analyzing telegram of water meter...");
    var result = await driver.AnalyzeTelegram(telegram, "auto", pass);
    Console.WriteLine("Result:");
    Console.WriteLine(JsonSerializer.Serialize(result.Item2, new JsonSerializerOptions { WriteIndented = true }));

    Console.WriteLine("Analyzing telegram of hca meter...");
    telegram = "32446850003076816980a0919f2b06007007000061087c08000000000000000000000000010101020100000000000000000000";
    var resultHca = await driver.AnalyzeTelegram(telegram);
    Console.WriteLine("Result:");
    Console.WriteLine(JsonSerializer.Serialize(resultHca.Item2, new JsonSerializerOptions { WriteIndented = true }));

    Console.WriteLine("Analyzing telegram of electricity meter...");
    telegram = "4E4401061010101002027A00004005_2F2F0E035040691500000B2B300300066D00790C7423400C78371204860BABC8FC100000000E833C8074000000000BAB3C0000000AFDC9FC0136022F2F2F2F2F";
    var resultElectricity = await driver.AnalyzeTelegram(telegram);
    Console.WriteLine("Result:");
    Console.WriteLine(JsonSerializer.Serialize(resultElectricity.Item2, new JsonSerializerOptions { WriteIndented = true }));

    Console.WriteLine("Analyzing telegram of gas meter...");
    telegram = "2E44A5119870659930037A060020052F2F_0C933E842784060A3B00000A5A5901C4016D3B37DF2CCC01933E24032606";
    var resultGas = await driver.AnalyzeTelegram(telegram);
    Console.WriteLine("Result:");
    Console.WriteLine(JsonSerializer.Serialize(resultGas.Item2, new JsonSerializerOptions { WriteIndented = true }));


    Console.WriteLine("Press enter to quit...");
    Console.ReadLine();
}
#endregion

#region HIOSimulatorExample

if (HIO_SIMULATOR_TEST)
{
    var simulator = new StandardContinuousSimulator<ChesterRadonCloudMessage>()
                            .WithName("RadonSimulator")
                            .WithInterval(1000);

    simulator.OnDataGenerated += (sender, args) =>
    {
        Console.WriteLine("______________________________________________________________________");
        Console.WriteLine($"Simulator Name: {args.SimulatorName}, Message Id: {args.MessageId}, Timestamp: {args.Timestamp}");
        Console.WriteLine($"\n\tMessage\n\t: {args.Message}\n\n");
        Console.WriteLine("______________________________________________________________________");
    };
    
    var simTask = simulator.Start();
    // Quit when key is pressed
    Console.WriteLine("\nPress any key to stop simulation...\n");
    Console.ReadKey();
    simulator.Stop();

    await Task.WhenAny(new Task[] { simTask });

    Console.WriteLine("Press enter to quit...");
    Console.ReadLine();
}

#endregion

#region HIOSimulatorHandlerExample

if (HIO_SIMULATOR_HANDLER_TEST)
{
    var simulatorHandler = new SimulatorHandler();

    var receivedMessagesCount = 0;
    var receivedMessagesCountLock = new object();
    var numOfSimulators = 1000;
    var receivedMessagesPrintCycle = numOfSimulators * 10;
    var receivedMessagesPrintCycleCounter = 0;
    var lastPrintTime = DateTime.Now;

    simulatorHandler.OnDataGenerated += (sender, args) =>
    {
        /*
        Console.WriteLine("______________________________________________________________________");
        Console.WriteLine($"Simulator Name: {args.SimulatorName}, Message Id: {args.MessageId}, Timestamp: {args.Timestamp}");
        Console.WriteLine($"\n\tMessage\n\t: {args.Message}\n\n");
        Console.WriteLine("______________________________________________________________________");
        */
        lock(receivedMessagesCountLock)
        {
            receivedMessagesPrintCycleCounter++;
            receivedMessagesCount++;
            if (receivedMessagesPrintCycleCounter >= receivedMessagesPrintCycle)
            {
                Console.WriteLine($"Received Messages Count: {receivedMessagesCount}");
                Console.WriteLine($"Messages per second: {receivedMessagesPrintCycle / (DateTime.Now - lastPrintTime).TotalSeconds}");
                lastPrintTime = DateTime.Now;
                Console.WriteLine($"Number of running simulators: {simulatorHandler.NumberOfRunningSimulators}");
                receivedMessagesPrintCycleCounter = 0;
            }
        }
    };

    var simulatorId1 = simulatorHandler.AddNewSimulator(typeof(ChesterRadonCloudMessage), 
                                                        5000, 
                                                        "RadonSimulator", 
                                                        "Radon measurement simulation",
                                                        SerialNumber:BaseSimulator.GenerateSerialNumberString());

    var simulatorId2 = simulatorHandler.AddNewSimulator(typeof(ChesterClimeCloudMessage), 
                                                        7000, 
                                                        "ClimeSimulator", 
                                                        "Temperature measurement simulation",
                                                        SerialNumber:BaseSimulator.GenerateSerialNumberString());

    for(var i = 0; i < numOfSimulators; i++)
    {
        var sim = simulatorHandler.AddNewSimulator(typeof(ChesterRadonCloudMessage), 1000, $"RadonSimulator{i}", $"Radon measurement simulation {i}");
    }

    var keyPressTask = Task.Run(async () =>
    {
        Console.WriteLine("\nPress key to stop all simulators...");
        Console.ReadKey();
        Console.WriteLine("\nStopping simulators...");
        await simulatorHandler.StopAllSimulators();
    });

    // await handler since all simulators are still running
    await simulatorHandler.MonitorSimulations();

    Console.WriteLine("Press enter to quit...");
    Console.ReadLine();
}

#endregion

Console.WriteLine("Program ends. Goodbye");
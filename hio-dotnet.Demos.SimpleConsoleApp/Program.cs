using hio_dotnet.HWDrivers.Enums;
using hio_dotnet.HWDrivers.JLink;
using hio_dotnet.HWDrivers.MCU;

#region JLinkExample

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

#endregion
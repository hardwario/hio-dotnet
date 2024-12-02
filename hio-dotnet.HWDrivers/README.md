# Hardware Drivers

Hardware drivers contains drivers related to some specific hardware such as programmer [SEGGER JLink](https://www.segger.com/downloads/jlink/) or [Nordic Semiconductors Power Profiler Kit II](https://www.nordicsemi.com/Products/Development-hardware/Power-Profiler-Kit-2). Both of those HW tools are part of the [CHESTER DevKit](). 

## J-Link

J-Link allows to communicate with the different types of the microcontrollers (MCU). It can provide the layer to upload the firmware or provide the communication for communication and debugging your firmware application. You can use it for main communication with your hardware to upload some settings or recevie some information from the device during the lifetime or you can use it for communication during automated testing of your device in the stage of the testing.

The J-Link is usually connected with the device via small 10 pin connector. Please asure that you connected the J-Link with the target MCU. On the other side you must connect J-Link to the computer with USB connector. The MCU must have power supply if you want to communicate with it. 

The J-Link comes with the prepared "DLL" library to control the process of connection, communication and firmware upload. The HARDWARIO SDK HW Drivers Library wraps this DLL to provide high level interface to those functions. 

### Requirements
The DLL must be present in the program folder in this path:

```bash
./JLink/Driver/JLink_x64.dll
```

The project includes those drivers DLLs and they are set to be copied in the final output folder automatically. If your program crash, please run it from console to see information about the missing libraries or check the presence of the DLL manually in the output program folder.

### Getting connected J-Link

You can have multiple J-Link devices connected to the same computer. It happens often in the testers where you have one J-Link connected to the main MCU and other one to the modem for example. Then you need to search for available J-Links and get their serial numbers. You can do it like this:

```csharp
// Get all available connected JLinks
Console.WriteLine("Searching for available JLinks...");
var connected_jlinks = JLinkDriver.GetConnectedJLinks();

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
```

### Instancing the Driver
If you want to use just one you do not need to care about the serial number, but you still need it for instancing the MCUConsole class. Then you can take the first found J-Link serial number. There are two ways how to establish J-Link RTT communication. You can instance each channel separatelly with the MCUConsole class or use the combined console MCUMultiRTTConsole. We recommend to use MCUMultiRTTConsole because it works with the channels in the same thread and it prevents the access to the DLL library from parallel threads:

```csharp
// Take first available JLink
var devsn = connected_jlinks[0].SerialNumber.ToString();

// Create MCUConsole instance for Config and Log RTT channels
Console.WriteLine("JLink RTT Console is Starting :)\n\n");

var multiconsole = new MCUMultiRTTConsole(new List<MultiRTTClientBase>()
{
    new MultiRTTClientBase(){ Channel = 0, DriverType= RTTDriverType.JLinkRTT, Name = "ConfigConsole" },
    new MultiRTTClientBase(){ Channel = 1, DriverType= RTTDriverType.JLinkRTT, Name = "LogConsole" }
}, "nRF52840_xxAA", 4000, "mcumulticonsole", devsn);
```

### Subscribing to the new lines
When you created MCUMultiRTTConsole you can subscribe to the event with new data line:

```csharp
// Subscribe to NewRTTMessageLineReceived event to get RTT messages and set output to console
multiconsole.NewRTTMessageLineReceived += (sender, data) =>
{
    if (data?.Item2.Channel == 0)
        Console.WriteLine($"MCU Config Message: {data.Item1}");
    else if (data?.Item2.Channel == 1)
        Console.WriteLine($"MCU Log Message: {data.Item1}");
};
```

Then you can start listening:

```csharp
var cts = new CancellationTokenSource();
Task listeningTask = multiconsole.StartListening(cts.Token);
// Quit when key is pressed
Console.WriteLine("\nPress any key to stop listening...\n");
Console.ReadKey();
cts.Cancel();

await Task.WhenAny(new Task[] { listeningTask });
```

### Sending the commands
Whenever you need to send some command to the MCU you can call SendCommand function like this:

```csharp
// Try to send some command to the MCU
var command = "config show";
Console.WriteLine("Sending Command: " + command);
await multiconsole.SendCommand(0, command);
```
The "0" parameter selects the RTT channel. 

### Firmware upload
The MCUMultiRTTConsole contains the function for firmware uploading. If you are using the CHESTER device you can use the HioFirmwareDownloader static class to download the .hex file and then upload it like this:

```csharp
// chester current firmware
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
```

As you can see the console must be turned off before the process of the loading of the firmware and then reconnected again.

### Error propagation
J-Link DLL provides different types of the errors in decoded form. For example very often error is missing power supply for the target MCU. Those errors are reimplemented in the library and you will receive them as the Exception with the text description what type error happened. All main errors are described in the [JLinkErrorCodes]() enum.

## Power Profiler Kit II
Power Profiler Kit II helps to provide power supply for your MCU mainly during the development stage. It is advanced PC controlled power supply with the precise measurement of the output current. This helps especially with the development of the low power devices. This hardware does not come with some specific DLL. It comes just with the API available via COM port. This API is wrapped in the library to provide high level interface.

### Getting the PPKII
To get all available PPKII devices you can call:

```csharp
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
```
In this example we will use just one device, so we will take the first one:

```csharp 
// Select the first available PPK2 device
var selectedDevice = devices[0];
Console.WriteLine($"\nUsing PPK2 device on COM Port: {selectedDevice.PortName} with Serial Number: {selectedDevice.SerialNumber}");
```

### Instancing the driver
With the known port of the device you can instance the PPK2_Driver:

```csharp
var ppk2 = new PPK2_Driver(selectedDevice.PortName);
```

Then you can call the specific commands like setting up the voltage, turn it on/off or start measurement.

### Set up the output voltage

Setting of output voltage is in the milivolts:

```csharp
int voltage = 3300;
Console.WriteLine($"Setting source voltage to {voltage} mV...");
ppk2.SetSourceVoltage(voltage);
```

### Turn ouput on and off

To turn on the output voltage you can call:

```csharp
Console.WriteLine("Turning on DUT power...");
ppk2.ToggleDUTPower(PPK2_OutputState.ON);
```

To turn off the output voltage you can call:

```csharp
Console.WriteLine("Turning off DUT power...");
ppk2.ToggleDUTPower(PPK2_OutputState.OFF);
```

### Measurement
If you want to measure the output current you need to setup "source meter mode" first:

```csharp
Console.WriteLine("Switching to source meter mode...");
ppk2.UseSourceMeter();
```

Then you can use two modes how to measure the data. First is more simple one with storing the data into the file:

```csharp
var duration = 2000; // Time of measurement in miliseconds (2 seconds)
var filename = "captureddata.csv";

Console.WriteLine("Starting measurement...");
ppk2.StartMeasuring();

await ppk2.CaptureMeasurement(filename, duration, addTimestampPrefix: true, useDotForDecimals: false);

Console.WriteLine("Stopping measurement...");
ppk2.StopMeasuring();
```

If you will set the "addTimestampPrefix" option, the function will add timestamp as prefix to the filename automatically. If this prefix is not set, it will happen only when the file already exists. 
If you want to force the using '.' for decimal nubers you can set the "useDotForDecimals". Otherwise the decimals divider will be set based on your local language settings (for example ',' in the Czech Republic).

Another option how to work with measured data is managing the processing of the data by yourself in the loop:

```csharp
// Start measuring
Console.WriteLine("Starting measurement...");
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

Console.WriteLine("Stopping measurement...");
ppk2.StopMeasuring();
```

You can see two main steps above: Function GetData will get buffered data and Function Process data will convert them to the μA values. Then you will have those data in the List and you can process them any way you like.


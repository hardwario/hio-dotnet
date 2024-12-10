# Simple Console App Demo

This projects helps to explore parts of the HARDWARIO .NET SDK from the view of the console application level. 
This readme will not explain all the functions of each demo in details because they are explained in the projects readmes. 

## Start demo

Each demo in this console app is wrapped in separated region define. In each of those regions you will find the "if" statement which turns on/off specific demo in the console app. All variables for turning on/off of the demos are declared in start of the file [Program.cs](). If you would like to run some demo you need to change the specific variable from "false" to "true". For example if you would like to turn on the demo for HARDWARIO Simulator you need to setup this variable:

```
var HIO_SIMULATOR_TEST = true;
```

Then you can build and run the project. If you are in the folder with the project you can simply call:

```
dotnet run --configuration Debug
```

## Demos in the Console App

- [JLinkExample](./hio-dotnet.Demos.SimpleConsoleApp/Program.cs#L52) - Example of the communication with two independent channels via [SEGGER JLink](https://www.segger.com/downloads/jlink/). This demo includes basic communication via RTT channels and uploading the Firmware via JLink to the [CHESTER](https://www.hardwario.com/chester) device. You will need the connected JLink to run this example. Check that your device has power supply on before running this demo.
- [JLinkCombinedConsoleExample](./hio-dotnet.Demos.SimpleConsoleApp/Program.cs#L149) - This example uses the combined Multi RTT console class to handle the communication on two RTT channels. This is more recommended solution becuase class solves internally possible async conflicts of both channels. You will need the connected JLink to run this example. Check that your device has power supply on before running this demo.
- [PPK2Example](./hio-dotnet.Demos.SimpleConsoleApp/Program.cs#L295) - Example of the using the [Nordic Semiconductor Power Profiler Kit II](https://www.nordicsemi.com/Products/Development-hardware/Power-Profiler-Kit-2) driver. You can find the PPK2 automatically, connect it based on serial number, control the voltage, turn on/off voltage and measure the current and save it to the file. You will need connected PPK2 to run this demo. 
- [ChirpstackExample](./hio-dotnet.Demos.SimpleConsoleApp/Program.cs#L423) - Example of communicating with the [Chirpstack LoRaWAN Network Server](https://www.chirpstack.io/) via API. You can create applications, devices, register them, browse existing, etc. You need Chirpstack server running on your local/remote network. You will need to change the server IP address based on where your Chirpstack is running. Then you will need to use your own API token and Tenant Id to access the Chirpstack configuration.
- [ThingsBoardExample](./hio-dotnet.Demos.SimpleConsoleApp/Program.cs#L572) - Example of communicating with the [ThingsBoard IoT Server](https://thingsboard.io/) via API. You can create customers, devices, dashboards, upload/download telemetry data, etc. You need to run the ThingsBoard server on your local/remote network. If you use the localhost you can use pre-set baseUrl. If you use some remote server or specific port you need to change the baseUrl. You need to add your own device Id, username and password to run this demo.
- [HIOCloudV2Example](./hio-dotnet.Demos.SimpleConsoleApp/Program.cs#L746) - Basic example for [HARDWARIO Cloud v2](https://www.hardwario.com/cloud) API communication. You will need login information, api tokens, space and device id for running all HIOCloudV2 examples.
- [HIOCloudV2DownlinkExample](./hio-dotnet.Demos.SimpleConsoleApp/Program.cs#L863) - Example how to send downling message to Hardwario cloud so it can be downloaded by CHESTER.
- [HIOCloudV2AddDeviceAndConnectorExample](./hio-dotnet.Demos.SimpleConsoleApp/Program.cs#L885) - Example shows how to create device and connector in cloud and connect them with specific tag.
- [HIOCloudV2SimpleGrabberExample](./hio-dotnet.Demos.SimpleConsoleApp/Program.cs#L931) - Example shows how to run automatic grabber for the new messages from the cloud for one specific device. The grabber sends the event anytime when new message in cloud arrives.
- [HIOCloudV2SimpleGrabberHandlerExample](./hio-dotnet.Demos.SimpleConsoleApp/Program.cs#L971) - Example shows how to run automatic grabber for the new messages from the cloud for multiple parallel devices. 
- [HIO_WMBUSMETERExample](./hio-dotnet.Demos.SimpleConsoleApp/Program.cs#L1020) - Example for communication with the [wmbusmeters](https://github.com/wmbusmeters/wmbusmeters) instance which runs on HARDWARIO cloud via API. It shows how to decode wM-Bus telegrams from different types of Smart Meters.
- [HIOSimulatorExample](./hio-dotnet.Demos.SimpleConsoleApp/Program.cs#L1058) - Example of running one simple simulator of CHESTER Catalog Application data messages (same messages as you can receive from the HARDWARIO cloud).
- [HIOSimulatorHandlerExample](./hio-dotnet.Demos.SimpleConsoleApp/Program.cs#L1088) - Example of running multiple parallel simulators of CHESTER Catalog Application data messages.


### Requests for another examples

If you will need any other example please [contact us](https://www.hardwario.com/contact) or create issue for the request.
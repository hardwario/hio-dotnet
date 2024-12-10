# HARDWARIO Common Library

Hardwario .NET Common library contains basic enums and models for working with HARDWARIO CHESTER data and devices. There are models for serialization/deserialization of the device's messages and simulators to create simulated messages for testing of your application without need of connection to the real data source.

## Structure of the library

There are multiple namespaces in the library:

- Config - Configuration classes for specific device configurations
- Enums - Shared enums which wraps some of the settings parameters to the readable form
- Helpers - Helpers classes to wrap some operations that are used often such as time conversion
- Models - Data models
	- Catalog Apps - Data models of specific [CHESTER Catalog Applications](https://www.hardwario.com/chester/catalog-applications)
	- Common - Common models for the parts of the data messages shared across different catalog apps
	- DataSimulation - Simulation of the CHESTER messages

## Examples

### Working with the config classes
Config classes are actually two: [LoRaWANConfig](/hio-dotnet.Common/Config/LoRaWANConfig.cs) and [LTEConfig](/hio-dotnet.Common/Config/LTEConfig.cs). Later we will add the BLE config and App specific configs as well. 
Those classes helps to parse or create configuration for the CHESTER device. The CHESTER accept the config via console via cloud, JLink or BLE. This classes helps to take the string received from console and automatically parse it into the config property and back. 

Simple example you can see in this test:

```csharp
var config = new LoRaWANConfig()
    .WithDevAddr("12345678")
    .WithDevEui("87654321")
    .WithJoinEui("1111111111")
    .WithAppKey("2222222222")
    .WithAppSKey("3333333333")
    .WithNwkSKey("4444444444")
    .WithAntenna(AntennaType.External)
    .WithBand(LoRaWANBand.US915)
    .WithMode(LoRaWANMode.ABP)
    .WithNetwork(LoRaWANNetwork.Public)
    .WithClass(LoRaWANClass.C)
    .WithDutyCycle(true)
    .WithTest(true)
    .WithAdr(true)
    .WithDataRate(3);

var expectedConfig = "lrw config devaddr 12345678" + Environment.NewLine +
                     "lrw config deveui 87654321" + Environment.NewLine +
                     "lrw config joineui 1111111111" + Environment.NewLine +
                     "lrw config appkey 2222222222" + Environment.NewLine +
                     "lrw config appskey 3333333333" + Environment.NewLine +
                     "lrw config nwkskey 4444444444" + Environment.NewLine +
                     "lrw config antenna ext" + Environment.NewLine +
                     "lrw config band us915" + Environment.NewLine +
                     "lrw config mode abp" + Environment.NewLine +
                     "lrw config nwk public" + Environment.NewLine +
                     "lrw config class c" + Environment.NewLine +
                     "lrw config adr true" + Environment.NewLine +
                     "lrw config test true" + Environment.NewLine +
                     "lrw config dutycycle true" + Environment.NewLine +
                     "lrw config datarate 3";

Assert.Equal(expectedConfig, config.GetWholeConfig().Trim());
```

This is the complete setting which is available now. As you can see the config has function GetWholeConfig() which will return the all lines of configuration. You can request just one specific line like this:

```csharp
var line = config.GetConfigLine(nameof(config.DevAddr));
```

Same way you can parse just one line of some received string from console:

```csharp
var config = new LoRaWANConfig();
config.ParseLine("lrw config antenna internal");
```

The same logic works for the LTEConfig class. 

Please check the [LoRaWANConfig tests](/hio-dotnet.Tests.Common/LoRaWANConfigTests.cs) or [LTEConfig tests](/hio-dotnet.Tests.Common/LTEConfigTests.cs) for more details.

### Catalog Apps Models
The namespace [CatalogApps](/hio-dotnet.Common/Models/CatalogApps/) contains most of our [CHESTER Catalog Applications](https://www.hardwario.com/chester/catalog-applications) data models. 
You can use those models for serialization/deserialization of the cloud v2 messages bodies. If you do not know the type of the message you can use static class ChesterCloudMessageAutoIdentifier like this:

```csharp
var messages = await hiocloud.GetAllDeviceMessages(wall_space, specific_device);
var message = messages?.FirstOrDefault();
if (message.Type == HioCloudv2MessageType.Data)
{
	Type guessedType = ChesterCloudMessageAutoIdentifier.FindTypeByMessageStructure(message.Body);
	var parsed = System.Text.Json.JsonSerializer.Deserialize(message.Body, guessedType);
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
}
```

As you can see in the example the ChesterCloudMessageAutoIdentifier will guess the Type of the message and then you can use JsonSerializer to Deserialize the message Body to specific carrier class.

For creating empty CHESTER Message object you can use the ChesterCloudMessageFactory:

```csharp
var emptyMessage = ChesterCloudMessageFactory.GetChesterEmptyMessage(hio_dotnet.Common.Enums.DeviceType.CHESTER_CLIME_IAQ);
emptyMessage.Attribute.ProductName = "Chester Clime IAQ";
var emptyMessageJson = System.Text.Json.JsonSerializer.Serialize(emptyMessage, new System.Text.Json.JsonSerializerOptions
{
    WriteIndented = true // Print output nicely formatted
});
Console.WriteLine($"Serialized JSON Content:\n\n{emptyMessageJson}\n\n");
```
You can ask factory to create "Fake" message which includes the simulated data in the message:

```csharp
var simulatedMessage = ChesterCloudMessageFactory.GetChesterFakeMessage(hio_dotnet.Common.Enums.DeviceType.CHESTER_CLIME_IAQ);
var simulatedMessageJson = System.Text.Json.JsonSerializer.Serialize(simulatedMessage, new System.Text.Json.JsonSerializerOptions
{
    WriteIndented = true // Print output nicely formatted
});
Console.WriteLine($"Serialized JSON Content:\n\n{simulatedMessageJson}\n\n");
```

### Simple Simulator
This library contains message simulator mechanism which simplify creating the stream of the simulated messages. Each model for message contains Attributes which defines the parameters for simulation of the property value. You can see the Atrribute above each property in those models classes. It looks usually like this:

```csharp
[SimulationMeasurementAttribute(false, 10.0, 60.0, true, false, 0.02, 5)]
[JsonPropertyName("concentration_hourly")]
public SimpleDoubleMeasurementGroup ConcentrationHourly { get; set; } = new SimpleDoubleMeasurementGroup();
```
The [SimulationMeasurementAttribute](/hio-dotnet.Common/Models/DataSimulation/SimulationMeasurementAttribute.cs) defines information for simulation. In this case it means that the:

- "false" => Property is not static => must be simulated
- 10.0 => Minimum value
- 60.0 => Maximum value
- "true" => Needs to follow the previous simulated value in case that previous simulation data are provided
- "false" => Should raise => each next value in simulation will be lower than previous one. When the value will reach the bottom (Minimum value) it will go back to maximum and start falling again. In case of this is true it will do the oposite logic => raising and when maximum it goes to minimum and again. 
- 0.02 => maximum percentage change of the simulated value between simulation steps. 0.02 means 2%.
- 5 => number of items in the simulated list. This works only for the properties which are List like in the case above where "ConcentrationHourly" is type of "SimpleDoubleMeasurementGroup" which contains List of SimpleTimeDoubleMeasurement objects. After simulation process this list will contains 5 items.

Thanks to those Attributes you can get the simulated data simply with just calling this:

```csharp
var message = new ChesterRadonCloudMessage();
BaseSimulator.GetSimulatedData(message);
```
This will give you one message with statically generated simulated data in all properties which has defined the attribute. 

If you need to run stream of simulated messages you can use [StandardContinuousSimulator](/hio-dotnet.Common/Models/DataSimulation/StandardContinuousSimulator.cs) class like this:

First create instance of specific simulator with defined type of the message name and interval:

```csharp
 var simulator = new StandardContinuousSimulator<ChesterRadonCloudMessage>()
                         .WithName("RadonSimulator")
                         .WithInterval(1000);
```

Then you can register the event of the new created data:

```csharp
simulator.OnDataGenerated += (sender, args) =>
{
    Console.WriteLine("______________________________________________________________________");
    Console.WriteLine($"Simulator Name: {args.SimulatorName}, Message Id: {args.MessageId}, Timestamp: {args.Timestamp}");
    Console.WriteLine($"\n\tMessage\n\t: {args.Message}\n\n");
    Console.WriteLine("______________________________________________________________________");
};
```
The simulator needs to be started as the Task like this:

```csharp
var simTask = simulator.Start();
// Quit when key is pressed
Console.WriteLine("\nPress any key to stop simulation...\n");
Console.ReadKey();
simulator.Stop();

await Task.WhenAny(new Task[] { simTask });

Console.WriteLine("Press enter to quit...");
Console.ReadLine();
```
If you would like to run multiple of simulators same time it is better to use the Simulator Handler.

### Simulator Handler
Simulator Handler allows to run multiple simulators simultaneously. The running the simulator handler is similar to the simple simulator. First you will create the class and add the simulators:

```csharp
var simulatorHandler = new SimulatorHandler();
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
```

Then you can register the event for receiving the new messages like this:

```csharp
simulatorHandler.OnDataGenerated += (sender, args) =>
{
    Console.WriteLine("______________________________________________________________________");
    Console.WriteLine($"Simulator Name: {args.SimulatorName}, Message Id: {args.MessageId}, Timestamp: {args.Timestamp}");
    Console.WriteLine($"\n\tMessage\n\t: {args.Message}\n\n");
    Console.WriteLine("______________________________________________________________________");
};
```

Then you can run the task to "MonitorSimulations":

```csharp
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
```
With this way you can run tens or even thousands of simulations in parallel. It is just limited by your hardware.
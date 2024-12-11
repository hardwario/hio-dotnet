# HARDWARIO Cloud API

This project is helps to communicate with the [HARDWARIO Cloud API](https://www.hardwario.com/cloud).

The project contains wrapper for API and some helper classes to obtain new messages from the cloud automatically. 


## Requirements
You will need the login to the HARDWARIO Cloud to use this library. You can use the email and password or you can use just API token. 
You can create your login on [HARDWARIO Cloud webpage](https://hardwario.cloud/). But without the active [CHESTER](https://www.hardwario.com/chester) device you will have no usefull data in your login. 

Note: If you will need to request names of the "Spaces" you will need the email and password because API token is always related to some specific "Space".

## Demos
You can find whole demo in the [SimpleConsoleApp](/hio-dotnet.Demos.SimpleConsoleApp/) project.

## How to use this library

### Structure of the HARDWARIO Cloud

At start it is important to mention the overal structure of the HARDWARIO Cloud. The basic hierarchy is like this.

- User
	- Space
		- Device
		- Connector
		- Tag


### Login to the cloud

There are two ways how to login to the cloud. You can use your email and password or you can use the API token. Using the email and password is not recommedned for most of the application because you are giving whole access to your cloud account. There is practically only one reason when you need to use email+password login. It is when you need to search multiple "Spaces", because the API tokens are related only to one specific Space. 

The login information you must pass when you creating the main HioCloudDriver instance:

#### Login with the email and password
```csharp
var email = "YOUR EMAIL";
var password = "YOUR PASSWORD";

HioCloudDriver hiocloud = null; 
if (email != "YOUR EMAIL" && password != "YOUR PASSWORD")
    hiocloud = new HioCloudDriver(HioCloudDriver.DefaultHardwarioCloudUrl, email, password);
```

#### Login with the API token
If you have API token which you can create in the HARDWARIO Cloud under specific space. You can use that to create instance of the HioCloudDriver:
```csharp
var apitoken = "YOUR API TOKEN IF YOU HAVE CREATED IT IN HIO CLOUD SPACE";
var hiocloud = new HioCloudDriver(HioCloudDriver.DefaultHardwarioCloudUrl, apitoken, true);
```

#### Searching the spaces
If you will use the login with email and password you can search all your spaces like this:

```csharp
if (hiocloud != null)
{
    var spaces = await hiocloud.GetSpaces();
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
```

#### Searching devices in the space
Once you have some specific space ID you can list all devices in that specific Space like this:

```csharp
// fill your own space id
var demo_space = new Guid("0189f3f4-9549-78ed-959c-cf4a0b28e28c");
var devices = await hiocloud.GetAllDevicesOfSpace(demo_space);

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
```

#### Getting Device details
This is how you can get the device details:

```csharp
 // fill your own device id
 var specific_device = new Guid("0190a1da-1a4d-7bdb-acb9-70a6c702789f");
 var device = await hiocloud.GetDevice(demo_space, specific_device);

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

```

#### Listing the messages of specific device
The getting messages is simple with this command:

```csharp
var messages = await hiocloud.GetAllDeviceMessages(wall_space, specific_device);
```

With this you will receive list of HioCloudMessage objects. This object contains parameter "Body" which includes the main data you are looking for (for example measured meteo parameters by CHESTER Meteo). If you would like to parse automatically this data you can use the Type identifier like this:

```csharp
if (messages != null)
{
    Console.WriteLine("\n");
    Console.WriteLine("Messages:");
    foreach (var m in messages)
    {
        Console.WriteLine($"Message: {m.Type}, Time: {m.CreatedAt}, \n\nPayload:\n {m.Body}\n\n");
        if (m.Type == HioCloudMessageType.Data)
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
```


#### Downlink messages
CHESTER Device is capable to download the prepared message from the cloud in the moment when it sends data to the cloud. To prepare this message you can use the Function AddNewDownlinkMessage:

```csharp
 var apitoken = "YOUR_API_TOKEN";
 var spaceid = "YOUR_SPACE_ID";
 var deviceid = "YOUR_DEVICE_ID";

 var hiocloudAPI = new HioCloudDriver(HioCloudDriver.DefaultHardwarioCloudUrl, apitoken, true);

 var data = new
 {
     button_1_state = 2,
     buzzer = 5
 };

 var res = await hiocloudAPI.AddNewDownlinkMessage(new Guid(spaceid), 
                                                   new Guid(deviceid), 
                                                   data, 
                                                   HioCloudMessageType.Data);
```

This example is for CHESTER Andon and it tells CHESTER that the button_1_state is in the stage "Accepted" and buzzer will do short beep to acknoledge the worker who requested some action via CHESTER Andon button.

#### Adding the device with connector

The HARDWARIO Cloud is kind of middleware between you final service and physical device. So, sometimes you need to deploy automatically the connection of device and some callback on the cloud. This can be done with the HioCloudDriver as well. 

If you need to add the device with the connector (callback) you need to connect them with specific tag. It is good practice to create tag first:

```csharp
 var tag = new HioCloudTag()
               .WithName("test-tag")
               .WithColor("#0000FF");

 Console.WriteLine("Creating new tag...");
 var newTag = await hiocloud.CreateTag(spaceid, tag);
 Console.WriteLine($"Tag created: {newTag.Id}");
```

Then you can create specific device. You should know your device Serial Number (SN) and Claim token, but for the case of the test you can use the generator of SN and Claim token:

```csharp
var device = new HioCloudDevice()
                 .WithName("test-device")
                 .WithSpaceId(spaceid)
                 .WithSerialNumber(BaseSimulator.GenerateSerialNumberString())
                 .WithToken(HioCloudDevice.GenerateClaimToken())
                 .WithTag(newTag);

Console.WriteLine("Creating new device...");
var newDevice = await hiocloud.CreateDevice(spaceid, device);
Console.WriteLine($"Device created: {newDevice.Id}");
```
The last step is creating the connector (callback). It is the http api callback automatically called everytime when the new message will arrive to the cloud from the device. If you are using the ThingsBoard to receive data from the HARDWARIO Cloud we integrated the function where you can just add the device token and it will create the transformation function for you:

```csharp
 var connector = new HioCloudConnector()
                     .WithName("test-connector")
                     .WithDirection("up")
                     .WithTag(newTag)
                     .WithTrigger("data")
                     .WithTrigger("session")
                     .WithThingsBoardConnectionToken(devicetoken);

 Console.WriteLine("Creating new connector...");
 var newConnector = await hiocloud.CreateConnector(spaceid, connector);
 Console.WriteLine("Connector created.");
```
If you are not using the ThingsBoard you must prepare own transformation function with specific link and add it to the HioCloudv2Connector property with name "Transformation" before you are sending request to the cloud to create the connector. The basic transformation function looks like this:

```javascript
function main(job)  {
	let body = job.message.body
	return  {
		"method":  "POST",
		"url":  "https://.....YOUR LINK...",
		"header":  {
			"Content-Type":  "application/json"
		},
		"data": body
	}
}

```

#### Simple grabber for one device

This project includes the way how to run automatic grabber of the messages. There is Simple grabber for one device and as you will see later the handler for grabbing multiple devices in parallel. Lets dive into grabbing of one device first:

```csharp
var apitoken = "YOUR_API_TOKEN";
var spaceid = "YOUR_SPACE_ID";
var deviceid = "YOUR_DEVICE_ID";

Console.WriteLine("Creating Grabber instance...");
var grabber = new SimpleCloudMessageGrabber(new Guid(spaceid), 
                                            new Guid(deviceid), 
                                            HioCloudDriver.DefaultHardwarioCloudUrl, 
                                            apitoken, 
                                            useapitoken:true)
                  .WithName("Meteo2MessagesGrabber")  
                  .WithInterval(60000);

```
First you need to instance SimpleCloudMessageGrabber. It requires the HioCloudDriver instance in constructor. You can inject the existing one or you can instance it just for this grabber as you can see in the example. 
The interval 60000 says that grabber will check the API each 60 seconds for possible new message. 

The grabber has event which fires when new message is available:

```csharp
grabber.OnNewDataReceived += (sender, data) =>
{
    if (data.Message != null)
    {
        Console.WriteLine($"Grabber: {(sender as SimpleCloudMessageGrabber)?.Name} received new message:\n");
        Console.WriteLine($"Device {data.Message.DeviceName} created new message at time: {data.Message.CreatedAt}, \nMessage: \n\n{data.Message.Body}\n\n");
    }
};
```

The grabber needs to be started to start grabbing. You can do it like this:

```csharp
Console.WriteLine("Start grabbing the data from the HARDWARIO Cloud....");
var simTask = grabber.Start();
// Quit when key is pressed
Console.WriteLine("\nPress any key to stop grabber...\n");
Console.ReadKey();
await grabber.Stop();

await Task.WhenAny(new Task[] { simTask });

Console.WriteLine("Press enter to quit...");
Console.ReadLine();
```
The grabber.Start() returns Task so you can add this in to the service handler if you run it as a server or you can simple await it as you can see in the example above. 

#### Grabber for multiple parallel devices
The grabber handler for multiple devices allows to run multiple grabbers same time in parallel. It simplify the handling of then under one instance. The using of the grabber handler is similar as the classic simple grabber:

```csharp
var apitoken = "YOUR_API_TOKEN";
var spaceid = "YOUR_SPACE_ID";
var deviceid = "YOUR_DEVICE_ID";
var deviceid2 = "YOUR_DEVICE_ID";

var grabberHandler = new CloudMessagesGrabbersHandler();

grabberHandler.AddNewGrabber(new Guid(spaceid), 
                             new Guid(deviceid), 
                             HioCloudDriver.DefaultHardwarioCloudUrl, 
                             60000, 
                             "Meteo1MessagesGrabber",
                             apitoken: apitoken);

grabberHandler.AddNewGrabber(new Guid(spaceid),
                             new Guid(deviceid2),
                             HioCloudDriver.DefaultHardwarioCloudUrl,
                             60000,
                             "Meteo2MessagesGrabber",
                             apitoken: apitoken);
```

First you will instance CloudMessagesGrabberHandler. Then you will add the Grabbers for each device. Then you can register the event for new messages:

```csharp
grabberHandler.OnNewDataReceived += (sender, data) =>
{
    if (data.Message != null)
    {
        Console.WriteLine($"Grabber: {(sender as SimpleCloudMessageGrabber)?.Name} received new message:\n"); 
        Console.WriteLine($"Device {data.Message.DeviceName} created new message at time: {data.Message.CreatedAt}, \nMessage: \n\n{data.Message.Body}\n\n");
    }
};
```

If you want to start and await the grabbers you will await the function MonitorGrabbers() which runs main task. It is good to run some task for StopAllGrabbers() before you quit the application. You can do it same as in the example with simple grabber or you can create separated task for waiting for the end like this:

```csharp
 var keyPressTask = Task.Run(async () =>
 {
     Console.WriteLine("\nPress key to stop all grabbers...");
     Console.ReadKey();
     Console.WriteLine("\nStopping grabbers...");
     await grabberHandler.StopAllGrabbers();
 });

 // await handler since all grabbers are still running
 await grabberHandler.MonitorGrabbers();

 Console.WriteLine("Press enter to quit...");
 Console.ReadLine();
```

#### Another examples

If you will need any other example or help with creating your own application you can create issue for that or you can [contact us](https://www.hardwario.com/contact).

## API Documentation
The library contains just main, mostly used commands right now. If you are interested in all possible API commands you can search [Swagger HARDWARIO Cloud documentation](https://api.hardwario.cloud/v2/documentation/index.html).
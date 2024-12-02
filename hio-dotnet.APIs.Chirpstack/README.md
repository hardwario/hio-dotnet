# ChirpStack API Driver
[ChirpStack](https://www.chirpstack.io/) is open-source LoRaWAN Network Server. We recommend to use this server together with our product [HARDWARIO EMBER](https://www.hardwario.com/ember). 

If you do not have instance of the ChirpStack you can install it based on [following steps](https://www.chirpstack.io/docs/getting-started/debian-ubuntu.html). The link is for installation on Debian/Ubuntu Linux. If you use different system please check the documentation for proper installation steps. You can run the ChirpStack from [Docker](https://www.chirpstack.io/docs/getting-started/docker.html) as well.

## Using the driver

### Login

The ChirpStack requires login for access to the API. The most often way is using the API token. You can obtain the API token inside of administration of the ChirpStack.

Once you have API token you can create the "ChirpStackDriver" instance like this:

```csharp
// This must point to the API interface.
var server = "http://192.168.1.132";
var apiToken = "YOUR_API_TOKEN";
var cs = new ChirpStackDriver(apiToken, server, 8080, false);
```

If you would like to list the application available in your ChirpStack instance you need to know the "tenant id". You can get this tenatn id via browser UI. 

```csharp
var tenantId = "52f14cd4-c6f1-4fbd-8f87-4025e1d49242";
var resp = await cs.ListApplications(tenantId);

Console.WriteLine("Total count of applications: " + resp.TotalCount.ToString());
Console.WriteLine("");
foreach (var app in resp.Result)
{
    Console.WriteLine($"AppName: {app.Name}, AppId: {app.Id}");
}
```

If you know the Application Id you can list its devices:

```csharp
Console.WriteLine("Listing devices...");
var devs = await cs.ListDevicesInApplication(app.Id);
foreach (var dev in devs.Result)
{
    Console.WriteLine($"DeviceName: {dev.Name}, DeviceEui: {dev.DevEui}");
}
```

Each device has multiple parameters such as JoinEUI or AppKey, NwkKey, etc. what you need for connection with it. You can obtain those parameters like this:

```csharp
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
```


If you want to create new device you will need to know the "Device Profile Id". You can copy paste it from your browser UI, or you can obtain it from the API:

```csharp
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
```

Once you have specific profile Id you can add the device. For this test purpose we will create new empty application first, because you need to add device under specific application Id:

```csharp
Console.WriteLine("Creating Application...");
var addapp = await cs.CreateApplication("testapp", tenantId, "just testing app");
Console.WriteLine($"Application created: {addapp.Id}");
```

Then you can add the device to this application:

```csharp
if (addapp != null && !string.IsNullOrEmpty(addapp.Id))
{
    Console.WriteLine("Creating Device...");
    var adddev = await cs.CreateDevice(ChirpStackDriver.GetRandomDevEUI(), "testdevice", addapp.Id, devprofId, ChirpStackDriver.GetRandomJoinEUI());
    Console.WriteLine($"Device created.");
}
```

Driver includes commands for deleting the devices and applications. This is useful for the case that you are doing some automatic testing of your devices and you need to clear the environment after the test of the specific device:

```csharp
Console.WriteLine("Deleting Device...");
var deldev = await cs.DeleteDevice(newdeveui);
Console.WriteLine($"Device deleted.");

Console.WriteLine("Deleting Application...");
var delapp = await cs.DeleteApplication(addapp.Id);
Console.WriteLine($"Application deleted.");
```

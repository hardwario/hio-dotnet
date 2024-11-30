# ThingsBoard API Driver

[ThingsBoard](https://thingsboard.io/) is application for managing IoT devices and creating Dashboards for the IoT devices. It simplify the process of creating dashboards for non-programmers people. 

## Installation of ThingsBoard
If you do not have instance of the ThingsBoard you can install it locally by following [those steps](https://thingsboard.io/docs/user-guide/install/installation-options/). For experimenting with this application it is best option to have own local installation. Optionally you can install it on some of your server as well.

HARDWARIO is hosting the ThingsBoard instance as well. If you are our client, please [contact us](https://www.hardwario.com/contact) to get more information about the access to the HARDWARIO ThingsBoard instance.

## Using the driver
The driver is wrapper of the ThingsBoard API. Actually we do not implement all the API commands because lots of them you will use in just specific cases. [Here](https://demo.thingsboard.io/swagger-ui/index.html#/) you can find the full specification of ThingsBoard API. If you will miss any command, please let us know and we can add it for you to the driver or you can add it and create pull request.

### Login

The basic use of the driver requires specification of the address where the instance is running and login information. There are multiple login levels:

- Main Administrator
- Tenant Administrator
- Customer

You will need the proper account based on what you want to do. Some of the actions are limited because of limited rights of specific level of the account.

If you want to create instance of the driver and login you can call this:

```csharp
var baseUrl = "http://localhost";
var username = "tenant@thingsboard.org";
var password = "tenant";

Console.WriteLine("Initializing driver and connection...");
var thingsBoardDriver = new ThingsBoardDriver(baseUrl, username, password, 8080);
Console.WriteLine("Connection initialized.");
```

The ThingsBoard driver will use the username and password to obtain JWT token for later use. This token has time-limited validity, so after some time (based on the settings on side of the ThingsBoard) you will need the login again.


### Creating Customer and Users
You can create Customer automatically like this:

```csharp 
using hio_dotnet.APIs.ThingsBoard.Models;

Console.WriteLine("\nCreating new Customer...");
var newCustomer = await thingsBoardDriver.CreateCustomerAsync(new CreateCustomerRequest()
{
    Title = "Test Customer",
});
```

The user can has multiple Users who access the dashboards and device information:

```csharp
using hio_dotnet.APIs.ThingsBoard.Models;

Console.WriteLine("Adding new user to the customer...");
var sendActivationEmail = false;
var newUser = await thingsBoardDriver.AddUserToCustomer(new User()
{
    Authority = "CUSTOMER_USER",
    CustomerId = newCustomer.Id,
    TenantId = newCustomer.TenantId,
    Email = "testemail@hardwario.com",
    FirstName = "Test",
    LastName = "Tester"
}, sendActivationEmail);

Console.WriteLine($"New Customer created: {newCustomer.Id.Id}");
```

New user will need to create password. The ThingsBoard can send to the user's email invitation link, but you need to setup this option first in the Main Admin account of ThingsBoard. You can find the details [here](https://thingsboard.io/docs/user-guide/ui/mail-settings/). Remember that you can use this feature only when you are hosting the ThingsBoard on some secured domain (https), especially if you use some standard mail providers such as Gmail. 
If you have proper setup of the email in the Things board you can set the "sendActivationEmail" option to "true" (whih is provided as parameter to the "AddUserToCustomer" function) and ThingsBoard will send the activation email to the new user automatically. 

### Creating the device
Creating the device requires specification of the device profile. You can just use the Guid of the specific profile or you can obtain those profiles from the API like this:

```csharp
Console.WriteLine("\nGetting device profiles...");
var deviceProfiles = await thingsBoardDriver.GetDeviceProfilesAsync();
Console.WriteLine("Device profiles obtained.");
foreach (var profile in deviceProfiles)
{
    Console.WriteLine($"Device Profile: {profile.Name}, Id: {profile.Id.Id}");
}

var defaultProfile = deviceProfiles.Where(p => p.Name == "default").FirstOrDefault();
```

Then you can create device based on this profile:

```csharp
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
    }
}
```

### Getting the Connection Token for Sending the Telemetry Data
If you want to send the Telemetry data to the device you will need to get "connectionToken". It is unique token for each device in the instance. This token you can use in the connector in callback in the HARDWARIO Cloud v2. 

```csharp
Console.WriteLine("Getting Connection Info...");
var connectionInfo = await thingsBoardDriver.GetDeviceConnectionInfoAsync(newDevice.Id.Id.ToString());
var connectionToken = string.Empty;
if (connectionInfo != null)
{
    Console.WriteLine($"\nExample Device Telemetry Request: {connectionInfo.Http.Http}\n");
    connectionToken = thingsBoardDriver.ParseConnectionToken(connectionInfo);
    Console.WriteLine($"Device Connection Token: {connectionToken}");
}
```

### Adding the dashboard
You can create the dashboards manually in the Graphical User Interface of the ThingsBoard. If you have lots of devices or you would like to automate this process you can use the API commands as well. You need to create the template dashboard and download it as JSON first. Then you can load it, change the Device ID as source in Widgets and add it to the instance via API. Here you can see those steps:

```csharp
var dashboard_filename = "chester_counter.json";
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
}

```

### Sending Telemetry data
If you would like to send some simulated data to the ThingsBoard to test the dashboard you can use the message simulators like this:

```csharp
Console.WriteLine("\nCreating Sample Data");
var counterdevice = new ChesterCounterCloudMessage();
BaseSimulator.GetSimulatedData(counterdevice);
Console.WriteLine($"Sending simulated data...");
var res = await thingsBoardDriver.SendTelemetryData(counterdevice, connectionToken);
Console.WriteLine("Sample data sent.");
```

### Getting Telemetry data from ThingsBoard
ThingsBoard can server as your application database. It can parse some specific data and store them together with the timestamps and managed access rights. This can help you to build your specific application because you can use ThingsBoard as huge part of your backend and then you can just obtain the data from the API like this:

```csharp
// Device ID. Use your device id
var deviceId = "9d6f6e10-5a6f-11ef-8883-e9c7bb918e65";

// Specify the keys you want to obtain. If multiple keys are required use ',' as separator
// be sure that device telemetry contains those keys otherwise put there some existing keys
var keys = "therm_temperature,hygro_humidity";

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
```

This example gives you the latest data for the specific device. If you would like to obtain some range of the data specified by the time range you can use "GetTelemetryDataForTimeRangeAsync" function like this:

```csharp
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
```
This example gives you the telemetry data for the last day. 


### Deleting the items
For the testing purposes you might need to delete the items after the test. API and the driver has implemented Delete commands for each item:

```csharp
Console.WriteLine("\n Deleting Dashboard...");
var delDashboard = await thingsBoardDriver.DeleteDashboardAsync(newDashboard.Id.Id.ToString());
Console.WriteLine($"Dashboard deleted.");

Console.WriteLine("\nDeleting Test device...");
var delDevice = await thingsBoardDriver.DeleteDeviceAsync(newDevice.Id.Id.ToString());
Console.WriteLine($"Device deleted.");

Console.WriteLine("Deleting User...");
var deleteUser = await thingsBoardDriver.DeleteCustomerUserAsync(newUser.Id.Id.ToString());
Console.WriteLine($"User deleted.");

Console.WriteLine("\nDeleting Customer...");
var delCustomer = await thingsBoardDriver.DeleteCustomerAsync(newCustomer.Id.Id.ToString());
Console.WriteLine($"Customer deleted.");
```

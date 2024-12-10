# Phone Drivers Library

This project contains phone related drivers. Actually it is the Bluetooth Low Energy (BLE) driver. 

## Shiny.BluetoothLE
The project uses external library [Shiny.BluetoothLE](https://shinylib.net/client/ble/manager/). The library manages the searching the BLE devices and connect to them. 
There is simple wrapper for this library to provide some high level functions to search CHESTER devices, connect to them, load the basic information or start communication on BLE console. The wrapper service is called [ChesterBLEService.cs](/hio-dotnet.PhoneDrivers/BLE/ChesterBLEService.cs).

## Using service in the MAUI application
There is example which use this driver called [hio-dotnet.Demos.HardwarioManager](/hio-dotnet.Demos.HardwarioManager/).

### Injecting the service
If you want to use the service in the [MAUI](https://dotnet.microsoft.com/en-us/apps/maui) application you must add it in the [MauiProgram.cs](/hio-dotnet.Demos.HardwarioManager/MauiProgram.cs#L25) first:

```csharp
using hio_dotnet.PhoneDrivers.BLE;
....
....
builder.Services.AddSingleton<ChesterBLEService>();
....

```
Then you can use this service in the components by injecting it into the component as you can see in the [Home.razor](/hio-dotnet.Demos.HardwarioManager/Components/Pages/Home.razor) component/page.

```csharp
@using hio_dotnet.PhoneDrivers.BLE
@inject ChesterBLEService BleService

...
protected override Task OnAfterRenderAsync(bool firstRender)
{
	if (firstRender) 
	{
		BleService.PeripherialsDictChanged += refresh;
		BleService.Connected += connected;
		BleService.DeviceDetialsLoaded += devicedetailsloaded;
		await BleService.ScanForDevices();
	}
}

```

### Connecting to the device

The events in this case providing the way how to refresh the UI because of new information about the available BLE devices which comes in the PeripheralsDict. This dictionary contains the IPeripheral compatible object which allows to call ConnectToDevice function like this:

```csharp
var per = BleService.PeripherialsDict[chester];
await BleService.ConnectToDevice(per);
```

If the connected device is CHESTER you can load all accessible data about the CHESTER from BLE characteristicts like this:

```csharp
await BleService.GetChesterDescriptionData(BleService.ConnectedPeripheral);
```

### Console communication
The example of the console communiation is in this [Console.razor](/hio-dotnet.Demos.HardwarioManager/Components/Pages/Console.razor) component/page.

If you expect that the device is already connected you can simply subscribe to the new line event:

```csharp
private List<string> output = new List<string>();

protected override void OnInitialized()
{
    BleService.NewConsoleLineOutputReceived += OnNewLineReceived;
}

private void OnNewLineReceived(object sender, string newLine)
{
    InvokeAsync(() =>
    {
        output.Add(newLine);
        StateHasChanged();
    });
}
```

And then send the command whenever you need:

```csharp
private async Task HandleSendCommand(string command)
{
    output.Add("> " + command);
    await BleService.SendCommand(BleService.ConnectedPeripheral, command.ToLower().Trim() + "\n");
}
```


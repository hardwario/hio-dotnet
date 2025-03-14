# Hardwario Monitor WASM

This application offers same features as [Hardwario Monitor MAUI](./hio-dotnet.Demos.HardwarioMonitor).
The main difference is that this app runs and WebAssembly. That is why it needs simple backend to connect HW related drivers.
To start this server please start [SimpleDriversServer](./hio-dotnet.Demos.SimpleDriversServer). 

This version can work on most of modern operating systems. 
It is better to build the SimpleDriverServer with LibSerialPort driver. 
You can select the driver in the [HW Drivers Project](./hio-dotnet.HWDrivers/hio-dotnet.HWDrivers.csproj) file.

```csharp
<DefineConstants>WINDOWS;SERIALPORT</DefineConstants>
// change to 
<DefineConstants>LINUX;LIBSERIALPORT</DefineConstants>
// or
<DefineConstants>MACOS;LIBSERIALPORT</DefineConstants>
```

Even the original SerialPort should work on all platforms we have observed some troubles on linux arm32 os. 

The version of OS is better to specify (even there is some conditions based on RID), because RID system still has some issues in Visual Studio 2022.
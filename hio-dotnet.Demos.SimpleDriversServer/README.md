# Simple Drivers Server

This application creates server that can connect J-Link and PPK. It provides access to HW peripherials for WASM applications.

## Server
Server is based on [EmbedIO](https://github.com/unosquare/embedio).

It creates:
- API endpoint
- Web static server
- Web Sockets 

## Structure

This server is very simple. All classes are defined [here](/hio-dotnet.HWDrivers/Server) as part of the HW Drivers. 

API and Web Socket has common [Controller](/hio-dotnet.HWDrivers/Server/DriversCommonController.cs). It means you can use same commands on API and Web Socket. Each has prepared client. 

## Usage
Main usage is now for application [hio-dotnet.Demos.BlazorComponents.RadzenLib.WASM](/hio-dotnet.Demos.BlazorComponents.RadzenLib.WASM).
In [RemoteConsoleService.cs](hio-dotnet.Demos.BlazorComponents.RadzenLib.WASM/Services/RemoteConsoleService.cs) you can see how to use the drivers server. 

## Hosting of WASM
The server can be used to host the WASM application. In that case publish the WASM app and add it to the server exe/dll folder. Than you will run the server like this:

```csharp
using hio_dotnet.HWDrivers.Server;

Console.WriteLine("Hello, lets start drivers server :)");
var server = new LocalDriversServer("./BlazorApp");
await server.Start();
```
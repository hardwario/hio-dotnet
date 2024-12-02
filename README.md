# HARDWARIO .NET SDK

HARDWARIO .NET SDK project is here to help you with creating your custom application around the HARDWARIO ecosystem of IoT devices and services. 

This SDK focuses to .NET C# developers mainly, but thanks to the simplicity and readibility of the C# it can help to programmers who uses different languages as well. 

SDK consists of several libraries and example projects which will be described below. 

Please note that HARDWARIO .NET SDK is still under the development. We are happy to receive tips and bugreports if you would like to help us.

## Requirements

HARDWARIO .NET SDK is created in [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (tested in actual Version 17.12.1) with [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (updated to .NET 9 is in roadmap). 

Compilation and building MAUI applications for Android requires Installlation of [Android SDK](https://visualstudio.microsoft.com/vs/android/).

We recommend to use Windows machine for the development, but project can be build on MacOS and Linux as well because the .NET 8 is multiplatform framework. Applications written in .NET 8 C# can run on all platforms (Windows, MacOS, Linux). [MAUI](https://dotnet.microsoft.com/en-us/apps/maui) Applications can run also on Android and iOS devices.

UI Library uses [Radzen Component Library](https://blazor.radzen.com/?theme=material3). More information about the using this component library you can find in [Readme file for Hio-DotNet UI Component Library]().

WebAssembly applications demos are part of the demo projects. You can run them in most of the modern web browsers without having the active web server. You need just webhosting for this type of applications. We usually test them in Chrome browser or Edge browser.

## First Steps

### Cloning the repository

First step is to clone the repository to some local folder in your computer. You can do it with the command:
```
git clone https://github.com/hardwario/hio-dotnet.git
```

Then you can enter the project folder:

```
cd hio-dotnet
```
where you will find the hio-dotnet.sln solution file. This file can be opened in the Visual Studio 2022.

### Build the demo console application

There are multiple demo applications. One of them is simple console application with multiple partial demos how to use parts of the SDK. All partial demos are turned off in default and you need to allow one or multiple of them by setting the proper variable to "true". Then you can run the build with command:

```
dotnet run --project "hio-dotnet.Demos.SimpleConsoleApp/hio-dotnet.Demos.SimpleConsoleApp.csp
roj" --configuration Debug
```

As it is written above, the project has no sub-demo allowed by default so if you will not change it in the [Program.cs]() it will just write this (in case you are running it on Windows machine):

```
Running on Windows
Program ends. Goodbye
```

To discover more about the parts of the [Simple Console App Demo Please Continue with reading Here]().

## Projects in the Solution

There are multiple projects in the solution:

### Demos

- [hio-dotnet.Demos.BlazorComponents.Radzen.WASM]() - Example WebAssembly app with the UI components with use of the Radzen Component Library
- [hio-dotnet.Demos.BlazorComponents.WASM]() - Example WebAssembly app with the own basic UI components
- [hio-dotnet.Demos.HardwarioManager]() - Example MAUI Blazor Hybrid App for phones which can search for and connect to [CHESTER]() device and communicate with it
- [hio-dotnet.Demos.HardwarioMonitor]() - Example MAUI Blazor Hybrid App for desktop which can search and connect [SEGGER JLink programmer](https://www.segger.com/downloads/jlink/), [Nordic Semiconductor Power Profiler Kit II](https://www.nordicsemi.com/Products/Development-hardware/Power-Profiler-Kit-2) and connect the CHESTER device.
- [hio-dotnet.Demos.SimpleConsoleApp]() - Simple console application with examples of using parts of the HARDWARIO .NET SDK.

### APIs

- [hio-dotnet.APIs.HioCloudv2]() - Wrapper library for Official HARDWARIO Cloud version 2 API. 
- [hio-dotnet.APIs.Chirpstack]() - Wrapper library for [Chirpstack](https://www.chirpstack.io/) LoRaWAN network Server API
- [hio-dotnet.APIs.ThingsBoard]() - Wrapper library for [ThingsBoard](https://thingsboard.io/) Application API
- [hio-dotnet.APIs.Wmbusmeters]() - Wrapper library for HARDWARIO instance of [wmbusmeters](https://github.com/wmbusmeters/wmbusmeters) application running on our server.

### UI

There are two basic UI Components library project in the HARDWARIO .NET SKD. One shows how you can create own wrappers for components to create own component library from scratch. Second (main) uses already existing component library called [Radzen](https://blazor.radzen.com/?theme=material3) to simplify the creating nice smooth responsive UIs.

- [hio-dotnet.UI.BlazorComponents]() - own UI components build from scratch
- [hio-dotnet.UI.BlazorComponents.Radzen]() - UI components build with use of Radzen component library

### Other Projects

- [hio-dotnet.Common]() - basic models, helpers and classes usefull across all other projects, this project includes models of all [CHESTER Catalog Applications](https://www.hardwario.com/chester/catalog-applications) output data models and other usefull classes.
- [hio-dotnet.HWDrivers]() - hardware dependent drivers such as [SEGGER JLink](https://www.segger.com/downloads/jlink/) and [Nordic Semiconductors Power Profiler Kit II](https://www.nordicsemi.com/Products/Development-hardware/Power-Profiler-Kit-2)
- [hio-dotnet.PhoneDrivers] - library with phone dependend drivers such as Bluetooth Low Energy driver.

### Test project

- [hio-dotnet.Tests.Common]() - main test project with use of [xUnit test framework](https://xunit.net/docs/getting-started/v2/netfx/visual-studio).

## License

This project is under standard MIT license.

## Contribution

We welcome any contribution to the project. There are multiple ways how to contribute:

- Testing and creating issues for bugs or features. 
- Improving the library with pull requests
- Bringing ideas for new features

## Do you need help?

If you need help please feel free to contact us via [form on our website](https://www.hardwario.com/contact).
﻿using hio_dotnet.HWDrivers.Server;

Console.WriteLine("Hello, lets start drivers server :)");

var server = new LocalDriversServer("./BlazorApp");

await server.Start();

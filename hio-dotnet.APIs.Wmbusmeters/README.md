# wM-Bus Meters Decoder

This project creates simple wrapper for using the API of wM-Bus Meters decoder service. It is based on the application [wmbusmeters](https://github.com/wmbusmeters/wmbusmeters). HARDWARIO runs this application on own server and providing API to access it to decode the wMBus decoders. It is related to our product [CHESTER wM-Bus](https://www.hardwario.com/chester/wm-bus).

## Calling the API

For calling the API you need to instance the driver first:

```csharp
var driver = new WMBusAPIDriver();
```

Actually we do not limit this API, but it might change in the future. Righ now you do not need any specific login or API key to access this API, but there is limit of number of requests per client, so if you will use it too often you client will be blocked. If you will need to use this API for larger amount of requests please [contact us](https://www.hardwario.com/contact).

For requesting the decoding of the wM-Bus telegram you will simply call the function "AnalyzeTelegram":

```csharp
Console.WriteLine("Analyzing telegram of hca meter...");
telegram = "32446850003076816980a0919f2b06007007000061087c08000000000000000000000000010101020100000000000000000000";
var resultHca = await driver.AnalyzeTelegram(telegram);
Console.WriteLine("Result:");
Console.WriteLine(JsonSerializer.Serialize(resultHca.Item2, new JsonSerializerOptions { WriteIndented = true }));
```
If you will not specify the driver type the wmbusmeters app will try to recognize it automatically. This is recomended to most of the decoding processes.

Sometimes meters uses the password. Even if the password is just "zeros". To specify password you can use this type of call:

```csharp
var telegram = "3e4401067075340605077aa90030853b0d89f380b805889c74e194c350b41ed1c59b4ec5565d0aa77fec0d5c5a51be5f8e238df7176a1bca55ca0b8bed8f5e";
var pass = "000000000000000000000000000000";

Console.WriteLine("Analyzing telegram of water meter...");
var result = await driver.AnalyzeTelegram(telegram, "auto", pass);
Console.WriteLine("Result:");
Console.WriteLine(JsonSerializer.Serialize(result.Item2, new JsonSerializerOptions { WriteIndented = true }));
```

Actually there are just few common classes of the data from the meters:

- Water meter
- Electricity meter
- Gas meter
- Heat Cost Allocator meter

They have common structure. If you will need any specific model please let us know in the issue or [contact us](https://www.hardwario.com/contact).
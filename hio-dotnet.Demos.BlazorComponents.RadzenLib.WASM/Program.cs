using hio_dotnet.Demos.BlazorComponents.RadzenLib.Services;
using hio_dotnet.Demos.BlazorComponents.RadzenLib;
using hio_dotnet.Demos.BlazorComponents.RadzenLib.WASM;
using hio_dotnet.UI.BlazorComponents.RadzenLib.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Microsoft.Extensions.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddRadzenComponents();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// load configuration file appsettings.json and deserialize to AppConfig
var config = builder.Configuration.Get<AppConfig>();

MainDataContext.Initialize(config);

builder.Services.AddScoped<HioCloudService>();
builder.Services.AddScoped<ThingsBoardService>(sp => new ThingsBoardService(baseUrl: config?.ThingsBoardBaseURL,
                                                                            port: config?.ThingsBoardBasePort ?? 8080,
                                                                            useDefaultLogin: config?.UseDefaultLoginForThingsBoard ?? false,
                                                                            defaultLogin: config?.DefaultLoginForThingsBoard,
                                                                            defaultPass: config?.DefaultPasswordForThingsBoard));
builder.Services.AddScoped<RemoteConsoleService>();
builder.Services.AddScoped<LoadingOverlayService>();
builder.Services.AddScoped<AppService>();

if (config != null)
{
    MainDataContext.Initialize(config);
}

await builder.Build().RunAsync();

using hio_dotnet.Demos.BlazorComponents.RadzenLib.Services;
using hio_dotnet.Demos.BlazorComponents.RadzenLib;
using hio_dotnet.Demos.BlazorComponents.RadzenLib.WASM;
using hio_dotnet.UI.BlazorComponents.RadzenLib.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddRadzenComponents();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<HioCloudService>();
builder.Services.AddScoped<ThingsBoardService>();
builder.Services.AddScoped<RemoteConsoleService>();
builder.Services.AddScoped<LoadingOverlayService>();

var config = new AppConfig()
{
    AppName = "Hardwario Monitor",
    AppVersion = "1.0.0",
};
    
if (config != null)
{
    MainDataContext.Initialize(config);
}

await builder.Build().RunAsync();

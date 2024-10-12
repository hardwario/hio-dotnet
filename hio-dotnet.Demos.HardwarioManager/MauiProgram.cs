using Microsoft.Extensions.Logging;
using Shiny;
using hio_dotnet.PhoneDrivers.BLE;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

namespace hio_dotnet.Demos.HardwarioManager
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseShiny()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services
                .AddBlazorise(options =>
                {
                    options.Immediate = true;
                })
                .AddBootstrap5Providers()
                .AddFontAwesomeIcons();

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddBluetoothLE();
            builder.Services.AddSingleton<ChesterBLEService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

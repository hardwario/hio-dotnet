using CommunityToolkit.Maui;
using hio_dotnet.Demos.HardwarioMonitor.Services;
using hio_dotnet.UI.BlazorComponents.RadzenLib.Services;
using Microsoft.Extensions.Logging;
using Radzen;
using System.Reflection;
using System.Text.Json;
using Microsoft.Maui.LifecycleEvents;
using hio_dotnet.Common.Config;

namespace hio_dotnet.Demos.HardwarioMonitor
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .ConfigureLifecycleEvents(lifecycle =>
                {
#if WINDOWS
                    lifecycle.AddWindows(windows =>
                    {
                        windows.OnClosed((window, e) =>
                        {
                            MainDataContext.StopJLink();
                        });
                    });
#endif
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddRadzenComponents();
            builder.Services.AddScoped<ConsoleService>();
            builder.Services.AddScoped<HioCloudService>();
            builder.Services.AddScoped<ThingsBoardService>();
            builder.Services.AddScoped<LoadingOverlayService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            ZephyrRTOSStandardCommands.LoadCommandsFromFile("zephyr_rtos_commands.txt");

            var configPath = Path.Combine(FileSystem.AppDataDirectory, "appconfig.json");

            if (!File.Exists(configPath))
            {
                var json = LoadEmbeddedResource("appconfig.json");
                File.WriteAllText(configPath, json);
            }

            var configJson = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<AppConfig>(configJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (config != null)
            {
                MainDataContext.Initialize(config);
            }

            return builder.Build(); ;
        }

        public static string LoadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = assembly.GetManifestResourceNames()
                                       .FirstOrDefault(name => name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));

            if (resourcePath == null)
            {
                throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
            }

            using var stream = assembly.GetManifestResourceStream(resourcePath);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}

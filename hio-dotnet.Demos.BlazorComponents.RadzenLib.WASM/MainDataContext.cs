
namespace hio_dotnet.Demos.BlazorComponents.RadzenLib
{
    public static class MainDataContext
    {
        public static AppConfig Config { get; set; } = new AppConfig();

        public static void Initialize(AppConfig config)
        {
            Config = config;
        }

        public static string NotificationPosition { get; set; } = "position: fixed; right: 20px; bottom: 20px; z-index: 1000;";

    }
}


using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Demos.HardwarioMonitor
{
    public static class MainDataContext
    {
        public static AppConfig Config { get; set; } = new AppConfig();

        public static void Initialize(AppConfig config)
        {
            Config = config;
        }

        public static string NotificationPosition { get; set; } = "position: fixed; right: 20px; bottom: 20px; z-index: 1000;";

        public static event EventHandler OnStopJLink;

        public static void StopJLink()
        {
            OnStopJLink?.Invoke(null, null);
        }
    }
}

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
    }
}

using hio_dotnet.Common.Config;
using hio_dotnet.UI.BlazorComponents.RadzenLib.Services;

namespace hio_dotnet.Demos.HardwarioMonitor.Services
{
    public class AppService
    {
        private readonly HioCloudService _hioCloudService;
        public AppService(HioCloudService hioCloudService)
        {
            _hioCloudService = hioCloudService;
        }

        public void RegisterHioCloudDefaultLogin()
        {
            if (MainDataContext.Config.UseDefaultLoginForHioCloud)
            {
                _hioCloudService.UseDefaultLogin = true;
                _hioCloudService.DefaultLogin = MainDataContext.Config.DefaultLoginForHioCloud;
                _hioCloudService.DefaultPassword = MainDataContext.Config.DefaultPasswordForHioCloud;
            }
            
        }

        public async Task LoadCommandsFromFile(string url)
        {
            await ZephyrRTOSStandardCommands.LoadCommandsFromFileViaHttp(url);
        }
    }
}

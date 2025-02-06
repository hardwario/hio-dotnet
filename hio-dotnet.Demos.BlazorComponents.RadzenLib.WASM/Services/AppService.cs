using hio_dotnet.UI.BlazorComponents.RadzenLib.Services;

namespace hio_dotnet.Demos.BlazorComponents.RadzenLib.Services
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
    }
}

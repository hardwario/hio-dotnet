using hio_dotnet.APIs.HioCloud;
using hio_dotnet.APIs.HioCloud.Models;
using Radzen;

namespace hio_dotnet.UI.BlazorComponents.Radzen.Services
{
    public class HioCloudService
    {
        private readonly NotificationService _notificationService;
        public HioCloudService(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private HioCloudDriver hioCloudDriver = null;

        public event EventHandler<string> OnLoggedIn;
        public event EventHandler<string> OnGetError;

        public bool IsInitializedWithUsername { get; set; } = false;
        public bool IsInitializedWithApiToken { get; set; } = false;

        public bool IsLoggedIn { get; set; } = false;

        public async Task InitHioCloudDriver(string username = null, string password = null)
        {
            if (hioCloudDriver == null)
            {
                await Task.Run(() =>
                {
                    hioCloudDriver = new HioCloudDriver(HioCloudDriver.DefaultHardwarioCloudUrl, username, password);
                    IsInitializedWithUsername = true;
                    IsInitializedWithApiToken = false;
                    IsLoggedIn = true;
                });
            }
        }

        public async Task InitHioCloudDriver(string apitoken = null)
        {
            if (hioCloudDriver == null)
            {
                await Task.Run(() =>
                {
                    hioCloudDriver = new HioCloudDriver(HioCloudDriver.DefaultHardwarioCloudUrl, apitoken, useapitoken: true);
                    IsInitializedWithUsername = false;
                    IsInitializedWithApiToken = true;
                    IsLoggedIn = true;
                });
            }
        }

        public async Task<List<HioCloudSpace>?> GetSpaces()
        {
            if (hioCloudDriver == null)
            {
                _notificationService.Notify(NotificationSeverity.Error, "HioCloudDriver not initialized");
                return null;
            }
            return await hioCloudDriver.GetSpaces();
        }

        public async Task<List<HioCloudDevice>?> GetDevices(string spaceId)
        {
            if (hioCloudDriver == null)
            {
                _notificationService.Notify(NotificationSeverity.Error, "HioCloudDriver not initialized");
                return null;
            }

            if (string.IsNullOrEmpty(spaceId))
            {
                _notificationService.Notify(NotificationSeverity.Error, "SpaceId is empty");
                return null;
            }

            return await hioCloudDriver.GetAllDevicesOfSpace(Guid.Parse(spaceId));
        }

        public async Task<List<HioCloudMessage>?> HioCloudMessages(string spaceId, string deviceId)
        {
            if (hioCloudDriver == null)
            {
                _notificationService.Notify(NotificationSeverity.Error, "HioCloudDriver not initialized");
                return null;
            }

            if (string.IsNullOrEmpty(spaceId))
            {
                _notificationService.Notify(NotificationSeverity.Error, "SpaceId is empty");
                return null;
            }

            if (string.IsNullOrEmpty(deviceId))
            {
                _notificationService.Notify(NotificationSeverity.Error, "DeviceId is empty");
                return null;
            }
            return await hioCloudDriver.GetAllDeviceMessages(Guid.Parse(spaceId), Guid.Parse(deviceId));
        }

        public async Task<HioCloudMessage?> GetHioCloudMessage(Guid spaceId, Guid messageId)
        {
            if (hioCloudDriver == null)
            {
                _notificationService.Notify(NotificationSeverity.Error, "HioCloudDriver not initialized");
                return null;
            }

            return await hioCloudDriver.GetMessage(spaceId, messageId);
        }
    }
}

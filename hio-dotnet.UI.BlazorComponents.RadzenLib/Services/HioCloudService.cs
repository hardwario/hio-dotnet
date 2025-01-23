using hio_dotnet.APIs.HioCloud;
using hio_dotnet.APIs.HioCloud.Models;
using hio_dotnet.UI.BlazorComponents.RadzenLib.CHESTER.HioCloud.Models;
using Radzen;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.Services
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

        public List<HioCloudSpace> HioCloudSpaces = new List<HioCloudSpace>();
        public List<Space> Spaces = new List<Space>();

        public List<OpenedTab> Tabs { get; set; } = new List<OpenedTab>();

        public async Task InitHioCloudDriver(string username = null, string password = null)
        {
            if (hioCloudDriver == null)
            {
                await Task.Run(async () =>
                {
                    hioCloudDriver = new HioCloudDriver(HioCloudDriver.DefaultHardwarioCloudUrl, username, password);
                    
                    _ = await hioCloudDriver.Login(username, password);
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

        public async Task<HioCloudDevice?> GetDevice(string spaceId, string deviceId)
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

            return await hioCloudDriver.GetDevice(Guid.Parse(spaceId), Guid.Parse(deviceId));
        }

        public async Task<List<HioCloudTag>?> LoadTags(string spaceId)
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
            var tags = await hioCloudDriver.GetTags(Guid.Parse(spaceId));
            var space = Spaces.FirstOrDefault(s => s.Id == Guid.Parse(spaceId));

            if (space != null)
            {
                if (space.Tags == null)
                    space.Tags = tags ?? new List<HioCloudTag>();
                else
                {
                    foreach (var tag in tags ?? new List<HioCloudTag>())
                    {
                        if (!space.Tags.Any(t => t.Id == tag.Id))
                        {
                            space.Tags.Add(tag);
                        }
                    }
                }
            }
            return tags;
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

        public async Task LoadSpaces()
        {
            HioCloudSpaces = await GetSpaces() ?? new List<HioCloudSpace>();
            foreach (var space in HioCloudSpaces)
            {
                Spaces.Add(new Space 
                { 
                    Id = space.Id ?? Guid.NewGuid(), 
                    Name = space.Name 
                });
            }
        }

        public async Task LoadSpaceDevices(string spaceId)
        {
            var devices = await GetDevices(spaceId) ?? new List<HioCloudDevice>();
            foreach (var device in devices)
            {
                var space = Spaces.FirstOrDefault(s => s.Id == Guid.Parse(spaceId));
                if (space != null)
                {
                    if (!space.Devices.Any(d => d.Id == device.Id))
                    {
                        space.Devices.Add(new Device 
                        { 
                            Id = device.Id ?? Guid.NewGuid(), 
                            SpaceId = space.Id, 
                            Name = device.Name, 
                            SerialNumber = device.SerialNumber,
                            SpaceName = space.Name 
                        });
                    }
                }
            }
        }

        public async Task LoadDeviceMessages(string spaceId, string deviceId)
        {
            var messages = await HioCloudMessages(spaceId, deviceId) ?? new List<HioCloudMessage>();
            foreach (var message in messages)
            {
                var device = Spaces.SelectMany(s => s.Devices).FirstOrDefault(d => d.Id == Guid.Parse(deviceId));
                if (device != null)
                {
                    if (!device.Messages.Any(m => m.Id == message.Id))
                    {
                        device.Messages.Add(new Message 
                        { 
                            Id = message.Id, 
                            DeviceId = device.Id, 
                            SpaceId = device.SpaceId, 
                            Text = message.CreatedAt.ToString(),
                            DeviceName = device.Name,
                            SpaceName = device.SpaceName
                        });
                    }
                }
            }
        }

        public async Task<bool> AddTag(string spaceId, HioCloudTag tag)
        {
            if (hioCloudDriver == null)
            {
                _notificationService.Notify(NotificationSeverity.Error, "HioCloudDriver not initialized");
                return false;
            }

            if (tag == null)
            {
                _notificationService.Notify(NotificationSeverity.Error, "Tag is empty");
                return false;
            }
            var res = await hioCloudDriver.CreateTag(Guid.Parse(spaceId), tag);
            if (res == null)
                return false;
            else
            {
                var space = Spaces.FirstOrDefault(s => s.Id == Guid.Parse(spaceId));
                if (space != null)
                {
                    if (space.Tags == null)
                        space.Tags = new List<HioCloudTag>();
                    if (!space.Tags.Any(t => t.Id == res.Id))
                        space.Tags.Add(res);
                }
                return true;
            }
        }

        public async Task<bool> AddConnector(string spaceId, HioCloudConnector connector)
        {
            if (hioCloudDriver == null)
            {
                _notificationService.Notify(NotificationSeverity.Error, "HioCloudDriver not initialized");
                return false;
            }

            if (connector == null)
            {
                _notificationService.Notify(NotificationSeverity.Error, "Connector is empty");
                return false;
            }
            var res = await hioCloudDriver.CreateConnector(Guid.Parse(spaceId), connector);
            if (res == null)
                return false;
            else
            {
                var space = Spaces.FirstOrDefault(s => s.Id == Guid.Parse(spaceId));
                if (space != null)
                {
                    if (space.Connectors == null)
                        space.Connectors = new List<HioCloudConnector>();
                    if (!space.Connectors.Any(t => t.Name == res.Name))
                        space.Connectors.Add(res);
                }
                return true;
            }
        }

        public async Task<bool> AddTagToDevice(string spaceId, HioCloudTag tag, HioCloudDevice device)
        {
            if (hioCloudDriver == null)
            {
                _notificationService.Notify(NotificationSeverity.Error, "HioCloudDriver not initialized");
                return false;
            }

            if (tag == null)
            {
                _notificationService.Notify(NotificationSeverity.Error, "Tag is empty");
                return false;
            }
            var res = await hioCloudDriver.AddTagToDevice(Guid.Parse(spaceId), tag, device);
            if (res == null)
                return false;
            else
            {
                return true;
            }
        }
    }
}

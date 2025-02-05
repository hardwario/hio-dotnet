using hio_dotnet.APIs.HioCloud;
using hio_dotnet.APIs.HioCloud.Models;
using hio_dotnet.APIs.ThingsBoard.Models;
using hio_dotnet.Common.Models;
using hio_dotnet.Common.Models.CatalogApps;
using hio_dotnet.Common.Models.Common;
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

        public async Task<List<HioCloudMessage>?> HioCloudMessages(string spaceId, string deviceId, int num_of_items = 20)
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
            return await hioCloudDriver.GetAllDeviceMessages(Guid.Parse(spaceId), Guid.Parse(deviceId), num_of_items);
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
                        space.Devices.Add(new CHESTER.HioCloud.Models.Device
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
                        device.Messages.Add(new CHESTER.HioCloud.Models.Message
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

        public async Task<List<HioCloudConnector>?> LoadConnectors(string spaceId)
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

            var conns = await hioCloudDriver.GetConnectors(Guid.Parse(spaceId));
            if (conns != null)
            {
                foreach (var conn in conns)
                {
                    var space = Spaces.FirstOrDefault(s => s.Id.ToString() == spaceId);
                    if (space != null)
                    {
                        if (!space.Connectors.Any(c => c.Name == conn.Name))
                        {
                            space.Connectors.Add(new HioCloudConnector
                            {
                                Direction = conn.Direction,
                                RetryDelays = conn.RetryDelays,
                                State = conn.State,
                                Tags = conn.Tags,
                                Transformation = conn.Transformation,
                                Triggers = conn.Triggers,
                                Type = conn.Type,
                                Name = conn.Name
                            });
                        }
                    }
                }
            }
            return conns;
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

        public async Task<bool> UpdateConnector(string spaceId, HioCloudConnector connector)
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
            var res = await hioCloudDriver.UpdateConnector(Guid.Parse(spaceId), connector);
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

        public async Task<Tuple<Type, List<string>>> GetPropertiesNamesForDeviceMessage_allpropswithindexes(string spaceId, string deviceId, Type? type, bool useForcedType = false)
        {
            if (hioCloudDriver == null)
            {
                _notificationService.Notify(NotificationSeverity.Error, "HioCloudDriver not initialized");
                return new Tuple<Type, List<string>>(typeof(ChesterCommonCloudMessage), new List<string>());
            }
            if (string.IsNullOrEmpty(deviceId))
            {
                _notificationService.Notify(NotificationSeverity.Error, "DeviceId is empty");
                return new Tuple<Type, List<string>>(typeof(ChesterCommonCloudMessage), new List<string>());
            }
            var msgs = await hioCloudDriver.GetAllDeviceMessages(Guid.Parse(spaceId), Guid.Parse(deviceId), 1);

            if (msgs != null)
            {
                var msg = msgs.FirstOrDefault();
                if (msg == null) return new Tuple<Type, List<string>>(typeof(ChesterCommonCloudMessage), new List<string>());

                var chm2type = SetType(type, msg, useForcedType);

                try
                {
                    var chm2 = System.Text.Json.JsonSerializer.Deserialize(msg.Body, chm2type);
                    var names = TimeStampFormatDataConverter.GetCombinedKeys(chm2);

                    var device = Spaces.SelectMany(s => s.Devices).FirstOrDefault(d => d.Id == Guid.Parse(deviceId));
                    if (device != null)
                    {
                        foreach (var name in names)
                        {
                            if (!device.PropsToInclude.ContainsKey(name))
                            {
                                device.PropsToInclude.Add(name, true);
                            }
                        }
                    }

                    return new Tuple<Type, List<string>>(chm2type, names);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return new Tuple<Type, List<string>>(typeof(ChesterCommonCloudMessage), new List<string>());

        }

        public async Task<Tuple<Type, List<string>>> GetPropertiesNamesForDeviceMessage(string spaceId, string deviceId, Type? type, bool useForcedType = false)
        {
            if (hioCloudDriver == null)
            {
                _notificationService.Notify(NotificationSeverity.Error, "HioCloudDriver not initialized");
                return new Tuple<Type, List<string>>(typeof(ChesterCommonCloudMessage), new List<string>());
            }
            if (string.IsNullOrEmpty(deviceId))
            {
                _notificationService.Notify(NotificationSeverity.Error, "DeviceId is empty");
                return new Tuple<Type, List<string>>(typeof(ChesterCommonCloudMessage), new List<string>());
            }
            var msgs = await hioCloudDriver.GetAllDeviceMessages(Guid.Parse(spaceId), Guid.Parse(deviceId), 1);

            if (msgs != null)
            {
                var msg = msgs.FirstOrDefault();
                if (msg == null) return new Tuple<Type, List<string>>(typeof(ChesterCommonCloudMessage), new List<string>());

                var chm2type = SetType(type, msg, useForcedType);

                try
                {
                    var chm2 = System.Text.Json.JsonSerializer.Deserialize(msg.Body, chm2type);
                    var names = TimeStampFormatDataConverter.GetJSActiveCode(chm2, returnJustNames: true).ToList();

                    var device = Spaces.SelectMany(s => s.Devices).FirstOrDefault(d => d.Id == Guid.Parse(deviceId));
                    if (device != null)
                    {
                        foreach (var name in names)
                        {
                            if (!device.PropsToInclude.ContainsKey(name))
                            {
                                device.PropsToInclude.Add(name, true);
                            }
                        }
                    }

                    return new Tuple<Type, List<string>>(chm2type, names);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return new Tuple<Type, List<string>>(typeof(ChesterCommonCloudMessage), new List<string>());

        }

        public async Task<string> GetPropertiesTimestampFormatJSCode(string spaceId, string deviceId, List<string>? propsToInclude = null, Type? type = null, bool useForcedType = false)
        {
            if (hioCloudDriver == null)
            {
                _notificationService.Notify(NotificationSeverity.Error, "HioCloudDriver not initialized");
                return string.Empty;
            }
            if (string.IsNullOrEmpty(deviceId))
            {
                _notificationService.Notify(NotificationSeverity.Error, "DeviceId is empty");
                return string.Empty;
            }
            var msgs = await hioCloudDriver.GetAllDeviceMessages(Guid.Parse(spaceId), Guid.Parse(deviceId), 1);

            var device = Spaces.SelectMany(s => s.Devices).FirstOrDefault(d => d.Id == Guid.Parse(deviceId));
            if (device != null && device.PropsToInclude != null && device.PropsToInclude.Count == 0)
            {
                _ = await GetPropertiesNamesForDeviceMessage(spaceId, deviceId, null);
            }

            if (device != null)
            {
                if (propsToInclude == null)
                {
                    propsToInclude = new List<string>();
                    foreach (var name in device.PropsToInclude.Where(p => p.Value))
                    {
                        if (!propsToInclude.Contains(name.Key))
                        {
                            propsToInclude.Add(name.Key);
                        }
                    }
                }
            }

            if (msgs != null)
            {
                var msg = msgs.FirstOrDefault();
                if (msg == null) return string.Empty;

                var chm2type = SetType(type, msg, useForcedType);

                try
                {
                    var chm2 = System.Text.Json.JsonSerializer.Deserialize(msg.Body, chm2type);
                    if (chm2 == null) return string.Empty;

                    var tsd = TimeStampFormatDataConverter.GetJSActiveCode(chm2, propsToInclude:propsToInclude).ToList();

                    var r = string.Join("\n", tsd);

                    return r;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return string.Empty;

        }

        public async Task<string> GetPropertiesTimestampFormatJSCode1(string spaceId, string deviceId, List<string>? propsToInclude = null, Type? type = null, bool useForcedType = false)
        {
            if (hioCloudDriver == null)
            {
                _notificationService.Notify(NotificationSeverity.Error, "HioCloudDriver not initialized");
                return string.Empty;
            }
            if (string.IsNullOrEmpty(deviceId))
            {
                _notificationService.Notify(NotificationSeverity.Error, "DeviceId is empty");
                return string.Empty;
            }
            var msgs = await hioCloudDriver.GetAllDeviceMessages(Guid.Parse(spaceId), Guid.Parse(deviceId), 1);

            var device = Spaces.SelectMany(s => s.Devices).FirstOrDefault(d => d.Id == Guid.Parse(deviceId));
            if (device != null && device.PropsToInclude != null && device.PropsToInclude.Count == 0)
            {
                _ = await GetPropertiesNamesForDeviceMessage(spaceId, deviceId, null);
            }

            if (device != null) {
                if (propsToInclude == null)
                {
                    propsToInclude = new List<string>();
                    foreach (var name in device.PropsToInclude.Where(p => p.Value))
                    {
                        if (!propsToInclude.Contains(name.Key))
                        {
                            propsToInclude.Add(name.Key);
                        }
                    }
                }
            }

            if (msgs != null)
            {
                var msg = msgs.FirstOrDefault();
                if (msg == null) return string.Empty;

                var chm2type = SetType(type, msg, useForcedType);

                try
                {
                    var chm2 = System.Text.Json.JsonSerializer.Deserialize(msg.Body, chm2type);
                    if (chm2 == null ) return string.Empty;

                    var tsd = TimeStampFormatDataConverter.GetCombinedTimeStampDataJSCode(chm2, propsToInclude, true);

                    return tsd;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return string.Empty;

        }

        public async Task<string> GetPropertiesTimestampFormatJSCodeSplitted(string spaceId, string deviceId, List<string>? propsToInclude = null, Type? type = null, bool useForcedType = false)
        {
            if (hioCloudDriver == null)
            {
                _notificationService.Notify(NotificationSeverity.Error, "HioCloudDriver not initialized");
                return string.Empty;
            }
            if (string.IsNullOrEmpty(deviceId))
            {
                _notificationService.Notify(NotificationSeverity.Error, "DeviceId is empty");
                return string.Empty;
            }
            var msgs = await hioCloudDriver.GetAllDeviceMessages(Guid.Parse(spaceId), Guid.Parse(deviceId), 1);

            var device = Spaces.SelectMany(s => s.Devices).FirstOrDefault(d => d.Id == Guid.Parse(deviceId));
            if (device != null && device.PropsToInclude != null && device.PropsToInclude.Count == 0)
            {
                _ = await GetPropertiesNamesForDeviceMessage(spaceId, deviceId, null);
            }

            if (device != null)
            {
                if (propsToInclude == null)
                {
                    propsToInclude = new List<string>();
                    foreach (var name in device.PropsToInclude.Where(p => p.Value))
                    {
                        if (!propsToInclude.Contains(name.Key))
                        {
                            propsToInclude.Add(name.Key);
                        }
                    }
                }
            }

            if (msgs != null)
            {
                var msg = msgs.FirstOrDefault();
                if (msg == null) return string.Empty;

                var chm2type = SetType(type, msg, useForcedType);

                try
                {
                    var chm2 = System.Text.Json.JsonSerializer.Deserialize(msg.Body, chm2type);
                    if (chm2 == null) return string.Empty;

                    var tsd = TimeStampFormatDataConverter.GetTimeStampDataJSCode(chm2, propsToInclude, true);

                    return tsd;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return string.Empty;

        }

        private Type? SetType(Type? type, HioCloudMessage msg, bool useForcedType)
        {
            Type? chm2type;
            if (!useForcedType)
            {
                chm2type = ChesterCloudMessageAutoIdentifier.FindTypeByMessageStructure(msg.Body);
            }
            else
            {
                if (type == null)
                {
                    chm2type = ChesterCloudMessageAutoIdentifier.FindTypeByMessageStructure(msg.Body);
                }
                else
                {
                    chm2type = type;
                }
            }

            return chm2type;
        }
    }
}

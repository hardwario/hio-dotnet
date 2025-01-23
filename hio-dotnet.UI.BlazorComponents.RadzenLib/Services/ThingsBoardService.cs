using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hio_dotnet.UI.BlazorComponents.RadzenLib.ThingsBoard.Models;
using hio_dotnet.APIs.HioCloud;
using hio_dotnet.APIs.ThingsBoard;
using hio_dotnet.APIs.ThingsBoard.Models;
using System.Collections.Concurrent;
using hio_dotnet.APIs.ThingsBoard.Models.Dashboards;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.Services
{
    public class ThingsBoardService
    {
        private ThingsBoardDriver _driver;
        private string _baseUrl;
        private int _port;

        private HioCloudDriver hioCloudDriver = null;

        public event EventHandler<string> OnLoggedIn;
        public event EventHandler<string> OnGetError;

        public bool IsInitializedWithUsername { get; set; } = false;
        public bool IsInitializedWithApiToken { get; set; } = false;

        public bool IsLoggedIn { get; private set; } = false;

        public User User { get; private set; }

        public List<IOpenedTab> Tabs { get; set; } = new List<IOpenedTab>();

        public ListableDevicesResponse DevicesListable { get; private set; }
        public ListableDashboardResponse ListableDashboardResponse { get; private set; }

        public Dictionary<Guid, List<string>> DevicesDataKeys = new Dictionary<Guid, List<string>>();

        public List<DeviceProfile> DeviceProfiles = new List<DeviceProfile>();
        public ThingsBoardService(string baseUrl = "https://thingsboard.hardwario.com", int port = 0)
        {
            if (port != 0 && port > 0)
            {
                _baseUrl = $"{baseUrl}:{port}";
            }
            else
            {
                _baseUrl = baseUrl;
            }
            _port = port;
            IsLoggedIn = false;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                _driver = new ThingsBoardDriver(_baseUrl, 0);
                var res = await _driver.Login(username, password);
                if (res != null)
                {
                    var user = await _driver.GetActiveUserInfo();
                    if (user != null)
                    {
                        User = user;
                    }
                    else
                    {
                        IsLoggedIn = false;
                        OnGetError?.Invoke(this, "Failed to get user info.");
                        return false;
                    }

                    IsLoggedIn = true;
                    OnLoggedIn?.Invoke(this, "Logged in successfully.");
                    return true;
                }
                else
                {
                    IsLoggedIn = false;
                    OnGetError?.Invoke(this, "Login failed.");
                    return false;
                }
            }
            catch
            {
                IsLoggedIn = false;
                return false;
            }
        }

        public async Task<bool> GetDevices()
        {
            if (!IsLoggedIn)
            {
                throw new InvalidOperationException("User is not authenticated.");
            }
            try
            {
                var devices = await _driver.GetTenantDevicesAsync();
                if (devices != null)
                {
                    DevicesListable = devices;
                    return true;
                }
                else
                {
                    OnGetError?.Invoke(this, "Failed to get devices.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot get Devices. Error: {ex.Message}.");
                return false;
            }
        }

        public ThingsBoardDriver GetDriver()
        {
            if (!IsLoggedIn)
            {
                throw new InvalidOperationException("User is not authenticated.");
            }
            return _driver;
        }

        public async Task<string> GetLatestData(Guid deviceId, string keys)
        {
            if (!IsLoggedIn)
            {
                return "User is not authenticated.";
            }
            if (deviceId != Guid.Empty)
            {
                var telemetry = await _driver.GetTelemetryDataAsync(deviceId.ToString(), keys);
                if (telemetry != null)
                {
                    return telemetry;
                }
                else
                {
                    return "No data";
                }
            }
            else
            {
                return "No data";
            }
        }

        public async Task<string> GetDataHistory(Guid deviceId, string keys, DateTime start, DateTime end)
        {
            if (!IsLoggedIn)
            {
                return "User is not authenticated.";
            }
            if (deviceId != Guid.Empty)
            {
                var telemetry = await _driver.GetTelemetryDataForTimeRangeAsync(deviceId.ToString(), keys, start, end);
                if (telemetry != null)
                {
                    return telemetry;
                }
                else
                {
                    return "No data";
                }
            }
            else
            {
                return "No data";
            }
        }

        public async Task<bool> GetDeviceKeys(Guid deviceId)
        {
            if (!IsLoggedIn)
            {
                return false;
            }
            if (deviceId != Guid.Empty)
            {
                var keys = await _driver.GetTelemetryDataKeysAsync(deviceId.ToString());
                if (keys != null)
                {
                    DevicesDataKeys.TryAdd(deviceId, keys);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public async Task<bool> LoadDeviceProfiles()
        {
            if (!IsLoggedIn)
            {
                return false;
            }
            var profiles = await _driver.GetDeviceProfilesAsync();
            if (profiles != null)
            {
                DeviceProfiles = profiles;
                return true;
            }
            return false;
        }

        public async Task<Device?> CreateDevice(CreateDeviceRequest request)
        {
            if (!IsLoggedIn)
            {
                return null;
            }
            var res = await _driver.CreateDeviceAsync(request);
            if (res != null)
            {
                return res;
            }
            return null;
        }

        public async Task<string> GetConnectionToken(string deviceId)
        {
            if (!IsLoggedIn)
            {
                return string.Empty;
            }
            var connectionInfo = await _driver.GetDeviceConnectionInfoAsync(deviceId);
            var connectionToken = string.Empty;
            if (connectionInfo != null)
            {
                connectionToken = _driver.ParseConnectionToken(connectionInfo);
                if (connectionToken != null)
                {
                    return connectionToken;
                }
            }
            return string.Empty;
        }

        public async Task<Dashboard?> CreateDashboard(CreateDashboardRequest dashboard)
        {
            if (!IsLoggedIn)
            {
                return null;
            }
            var newDashboard = await _driver.CreateDashboardAsync(dashboard);
            if (newDashboard != null)
            {
                return newDashboard;
            }
            return null;
        }

        public async Task<Customer?> CreateCustomer(CreateCustomerRequest customer)
        {
            if (!IsLoggedIn)
            {
                return null;
            }
            var res = await _driver.CreateCustomerAsync(customer);
            if (res != null)
            {
                return res;
            }
            return null;
        }

        public async Task<bool> RemoveCustomer(Guid customerId)
        {
            if (!IsLoggedIn)
            {
                return false;
            }
            var res = await _driver.DeleteCustomerAsync(customerId.ToString());
            if (res != null)
            {
                return true;
            }
            return false;
        }

        public async Task<ListableDashboardResponse?> GetAllDashboards()
        {
            if (!IsLoggedIn)
            {
                return null;
            }
            var dashboards = await _driver.GetTenantDashboardsAsync();
            if (dashboards != null)
            {
                ListableDashboardResponse = dashboards;
                return dashboards;
            }
            return null;
        }

        public async Task<Dashboard?> GetDashboard(string dashboardId)
        {
            if (!IsLoggedIn)
            {
                return null;
            }
            var dashboard = await _driver.GetTenantDashboardAsync(dashboardId);
            if (dashboard != null)
            {
                return dashboard;
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Chirpstack.Api;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

namespace hio_dotnet.APIs.Chirpstack
{
    public class ChirpStackDriver
    {
        public ChirpStackDriver(string apiToken, string addressBase = "localhost", int serverPort = 8080, bool withHttps = false)
        {
            if (addressBase == null)
                throw new ArgumentNullException("ChirpStack Address cannot be null.");
            else if (string.IsNullOrEmpty(addressBase) || string.IsNullOrWhiteSpace(addressBase))
                throw new ArgumentNullException("ChirpStack Address cannot be empty or whitespace.");
            else
                _serverAddressBase = addressBase.Replace("http://", string.Empty).Replace("https://", string.Empty);

            if (withHttps)
                _serverAddressBase = "https://" + _serverAddressBase;
            else
                _serverAddressBase = "http://" + _serverAddressBase;

            if (serverPort <= 0)
                throw new ArgumentOutOfRangeException("ChirpStack port canno be bellow 0 or 0");
            else
                _serverPort = serverPort;

            _serverAddressFull = $"{_serverAddressBase}:{_serverPort}";

            if (!string.IsNullOrEmpty(apiToken))
                _apiToken = apiToken;
            else
                throw new ArgumentException("ChirpStack API Token cannot be null or empty.");

            if (!buildClient())
                throw new Exception("Cannot build ChirpStack client with address: " + _serverAddressFull);

        }

        private string _serverAddressBase = string.Empty;
        private int _serverPort = 8080;
        private string _serverAddressFull = string.Empty;
        private string _apiToken = string.Empty;
        private DeviceService.DeviceServiceClient? _deviceClient = null;
        private ApplicationService.ApplicationServiceClient? _applicationClient = null;
        private DeviceProfileService.DeviceProfileServiceClient? _deviceProfileClient = null;

        // Define the API key meta-data.
        private Metadata? _authToken = null;

        private bool buildClient()
        {
            try
            {
                // Connect without using TLS.
                //using var channel = GrpcChannel.ForAddress(
                var channel = GrpcChannel.ForAddress(
                    address: _serverAddressFull,
                    channelOptions: new GrpcChannelOptions()
                    {
                        HttpHandler = new GrpcWebHandler(new HttpClientHandler()),
                        Credentials = ChannelCredentials.Insecure,
                    }
                );

                // Device-queue API client.
                _deviceClient = new DeviceService.DeviceServiceClient(channel);
                _applicationClient = new ApplicationService.ApplicationServiceClient(channel);
                _deviceProfileClient = new DeviceProfileService.DeviceProfileServiceClient(channel);

                _authToken = new Metadata
                {
                    { "Authorization", "Bearer " + _apiToken },
                };

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot build ChirpStack client with address: " + _serverAddressFull + ". Message: " + ex.Message);
                return false;
            }
        }

        #region Applications

        /// <summary>
        /// List all available applications
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ListApplicationsResponse?> ListApplications(string tenantId, uint limit = 10, uint offset = 0)
        {
            var req = new ListApplicationsRequest
            {
                TenantId = tenantId,
                Limit = limit,
                Offset = offset
            };

            if (_applicationClient != null)
            {
                var applicationsData = await _applicationClient.ListAsync(req, headers: _authToken);
                if (applicationsData != null)
                {
                    return applicationsData;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception("ListApplications>> ChirpStack Client is not initialized.");
            }
        }

        /// <summary>
        /// Get application details
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<GetApplicationResponse?> GetApplicationDetails(string applicationId)
        {
            if (string.IsNullOrEmpty(applicationId))
                throw new ArgumentNullException("GetApplicationDetails>> applicationId cannot be null or empty.");

            var req = new GetApplicationRequest
            {
                Id = applicationId
            };

            if (_applicationClient != null)
            {
                var appData = await _applicationClient.GetAsync(req, headers: _authToken);
                if (appData != null)
                {
                    return appData;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception("GetApplicationDetails>> ChirpStack Client is not initialized.");
            }
        }

        /// <summary>
        /// Create new application
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tenantId"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<CreateApplicationResponse?> CreateApplication(string name, string tenantId, string description = "")
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("CreateApplication>> name cannot be null or empty.");

            if (string.IsNullOrEmpty(tenantId))
                throw new ArgumentNullException("CreateApplication>> organizationId cannot be null or empty.");

            var req = new CreateApplicationRequest
            {
                Application = new Application
                {
                    Name = name,
                    TenantId = tenantId,
                    Description = description
                }
            };

            if (_applicationClient != null)
            {
                var createResponse = await _applicationClient.CreateAsync(req, headers: _authToken);
                if (createResponse != null)
                {
                    return createResponse;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception("CreateApplication>> ChirpStack Client is not initialized.");
            }
        }

        /// <summary>
        /// Delete application
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<Empty?> DeleteApplication(string applicationId)
        {
            if (string.IsNullOrEmpty(applicationId))
                throw new ArgumentNullException("DeleteApplication>> applicationId cannot be null or empty.");

            var req = new DeleteApplicationRequest
            {
                Id = applicationId
            };

            if (_applicationClient != null)
            {
                var deleteResponse = await _applicationClient.DeleteAsync(req, headers: _authToken);
                return deleteResponse;
            }
            else
            {
                throw new Exception("DeleteApplication>> ChirpStack Client is not initialized.");
            }
        }

        /// <summary>
        /// List all devices in application
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<ListDevicesResponse?> ListDevicesInApplication(string applicationId, uint limit = 10, uint offset = 0)
        {
            if (string.IsNullOrEmpty(applicationId))
                throw new ArgumentNullException("ListDevicesInApplication>> applicationId cannot be null or empty.");

            var req = new ListDevicesRequest
            {
                ApplicationId = applicationId,
                Limit = limit,
                Offset = offset
            };

            if (_deviceClient != null)
            {
                var devicesData = await _deviceClient.ListAsync(req, headers: _authToken);
                if (devicesData != null)
                {
                    return devicesData;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception("ListDevicesInApplication>> ChirpStack Client is not initialized.");
            }
        }

        #endregion

        #region Devices

        /// <summary>
        /// Get device details such as name, description, id, etc.
        /// </summary>
        /// <param name="deveui"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<GetDeviceResponse?> GetDeviceDetails(string deveui)
        {
            if (string.IsNullOrEmpty(deveui))
                throw new ArgumentNullException("GetDeviceDetail>> deveui cannot be null or empty.");

            var req = new GetDeviceRequest
            {
                DevEui = deveui
            };
            if (_deviceClient != null)
            {
                var deviceData = await _deviceClient.GetAsync(req, headers: _authToken);
                if (deviceData != null)
                {
                    return deviceData;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception("GetDeviceDetail>> Chirpstack Client is not initialized.");
            }
        }

        /// <summary>
        /// Get device keys such as NwkKey, AppKey, etc.
        /// </summary>
        /// <param name="deveui"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<GetDeviceKeysResponse?> GetDeviceKeys(string deveui)
        {
            if (string.IsNullOrEmpty(deveui))
                throw new ArgumentNullException("GetDeviceKeys>> deveui cannot be null or empty.");

            var req = new GetDeviceKeysRequest
            {
                DevEui = deveui
            };

            if (_deviceClient != null)
            {
                var deviceKeys = await _deviceClient.GetKeysAsync(req, headers: _authToken);
                if (deviceKeys != null)
                {
                    return deviceKeys;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception("GetDeviceKeys>> Chirpstack Client is not initialized.");
            }
        }

        /// <summary>
        /// Get device activation parameters such as network session key, app session key, etc.
        /// </summary>
        /// <param name="deveui"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<GetDeviceActivationResponse?> GetDeviceActivation(string deveui)
        {
            if (string.IsNullOrEmpty(deveui))
                throw new ArgumentNullException("GetDeviceActivation>> deveui cannot be null or empty.");

            var req = new GetDeviceActivationRequest
            {
                DevEui = deveui
            };

            if (_deviceClient != null)
            {
                var deviceActivationData = await _deviceClient.GetActivationAsync(req, headers: _authToken);
                if (deviceActivationData != null)
                {
                    return deviceActivationData;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception("GetDeviceActivation>> ChirpStack Client is not initialized.");
            }
        }

        /// <summary>
        /// Get specific device profile by ID
        /// </summary>
        /// <param name="deviceProfileId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<GetDeviceProfileResponse?> GetDeviceProfile(string deviceProfileId)
        {
            if (string.IsNullOrEmpty(deviceProfileId))
                throw new ArgumentNullException("GetDeviceProfile>> deviceProfileId cannot be null or empty.");

            var req = new GetDeviceProfileRequest
            {
                Id = deviceProfileId
            };

            if (_deviceProfileClient != null)
            {
                var profileData = await _deviceProfileClient.GetAsync(req, headers: _authToken);
                if (profileData != null)
                {
                    return profileData;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception("GetDeviceProfile>> ChirpStack Client is not initialized.");
            }
        }

        /// <summary>
        /// List all device profiles
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ListDeviceProfilesResponse?> ListDeviceProfiles(string tenantId)
        {

            var req = new ListDeviceProfilesRequest
            {
                Limit = 10,
                Offset = 0,
                TenantId = tenantId
            };

            if (_deviceProfileClient != null)
            {
                var profileData = await _deviceProfileClient.ListAsync(req, headers: _authToken);
                if (profileData != null)
                {
                    return profileData;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception("GetDeviceProfile>> ChirpStack Client is not initialized.");
            }
        }

        /// <summary>
        /// Delete device by specified devEui
        /// </summary>
        /// <param name="devEui"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<Empty> DeleteDevice(string devEui)
        {
            if (string.IsNullOrEmpty(devEui))
                throw new ArgumentNullException("DeleteDevice>> devEui cannot be null or empty.");

            var req = new DeleteDeviceRequest
            {
                DevEui = devEui
            };

            if (_deviceClient != null)
            {
                var response = await _deviceClient.DeleteAsync(req, headers: _authToken);
                return response;
            }
            else
            {
                throw new Exception("DeleteDevice>> ChirpStack Client is not initialized.");
            }
        }

        /// <summary>
        /// Create new device
        /// </summary>
        /// <param name="devEui">Use ChirpStackDriver.GetRandomDevEUI() to get random deveui</param>
        /// <param name="name"></param>
        /// <param name="applicationId">GUID of app</param>
        /// <param name="deviceProfileId">GUID of device profile</param>
        /// <param name="joineui">Use ChirpStackDriver.GetRandomJoinEUI() to get random joineui</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<Empty?> CreateDevice(string devEui, string name, string applicationId, string deviceProfileId, string joineui)
        {
            if (string.IsNullOrEmpty(devEui))
                throw new ArgumentNullException("CreateDevice>> devEui cannot be null or empty.");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("CreateDevice>> name cannot be null or empty.");

            if (string.IsNullOrEmpty(applicationId))
                throw new ArgumentNullException("CreateDevice>> applicationId cannot be null or empty.");

            if (string.IsNullOrEmpty(deviceProfileId))
                throw new ArgumentNullException("CreateDevice>> deviceProfileId cannot be null or empty.");

            var req = new CreateDeviceRequest
            {
                Device = new Device
                {
                    DevEui = devEui,
                    Name = name,
                    ApplicationId = applicationId,
                    DeviceProfileId = deviceProfileId,
                    JoinEui = joineui
                }
            };

            if (_deviceClient != null)
            {
                var createResponse = await _deviceClient.CreateAsync(req, headers: _authToken);
                if (createResponse != null)
                {
                    return createResponse;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new Exception("CreateDevice>> ChirpStack Client is not initialized.");
            }
        }


        #endregion

        #region Helpers

        /// <summary>
        /// Function to generate a random 8-byte EUI
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandomEUI()
        {
            byte[] euiBytes = new byte[8];
            Random random = new Random();
            random.NextBytes(euiBytes);

            // Convert the byte array to a hexadecimal string
            return BitConverter.ToString(euiBytes).Replace("-", "");
        }

        /// <summary>
        /// Function to get a random DevEUI
        /// </summary>
        /// <returns></returns>
        public static string GetRandomDevEUI()
        {
            return GenerateRandomEUI();
        }

        /// <summary>
        /// Function to get a random JoinEUI
        /// </summary>
        /// <returns></returns>
        public static string GetRandomJoinEUI()
        {
            return GenerateRandomEUI();
        }

        #endregion
    }
}

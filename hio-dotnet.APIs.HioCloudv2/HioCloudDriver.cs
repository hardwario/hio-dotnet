using hio_dotnet.APIs.HioCloud.Models;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace hio_dotnet.APIs.HioCloud
{
    public class HioCloudDriver
    {
        public static string DefaultHardwarioCloudUrl = HioCloudDefaults.DefaultHardwarioCloudUrl;

        /// <summary>
        /// Use this constructor to authenticate with ThingsBoard using username and password.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="port"></param>
        /// <exception cref="ArgumentException"></exception>
        public HioCloudDriver(string baseUrl, string username, string password, int port = 0)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("Base URL cannot be empty.");
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username cannot be empty.");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be empty.");

            _baseUrl = port > 0 ? $"{baseUrl}:{port}" : baseUrl;
            _port = port;
            //_jwtToken = GetAuthTokenAsync(_baseUrl, username, password).Result;
            _useapitoken = false;
        }

        /// <summary>
        /// Use this constructor to authenticate with ThingsBoard using JWT token.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="jwtToken"></param>
        /// <param name="port"></param>
        /// <exception cref="ArgumentException"></exception>
        public HioCloudDriver(string baseUrl, string jwtToken, int port = 0)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("Base URL cannot be empty.");
            if (string.IsNullOrEmpty(jwtToken))
                throw new ArgumentException("JWT token cannot be empty.");

            _baseUrl = baseUrl;
            _port = port;
            _jwtToken = jwtToken;
            _useapitoken = false;
        }

        /// <summary>
        /// Use this constructor to authenticate with ThingsBoard using API token.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="jwtToken"></param>
        /// <param name="port"></param>
        /// <exception cref="ArgumentException"></exception>
        public HioCloudDriver(string baseUrl, string apitoken, bool useapitoken = true, int port = 0)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("Base URL cannot be empty.");
            if (string.IsNullOrEmpty(apitoken))
                throw new ArgumentException("API token cannot be empty.");

            _baseUrl = baseUrl;
            _port = port;
            _apitoken = apitoken;
            _useapitoken = useapitoken;
        }

        private string _baseUrl = string.Empty;
        private int _port = 0;
        private string _jwtToken = string.Empty;
        private string _apitoken = string.Empty;
        private bool _useapitoken = false;

        /// <summary>
        /// This method is used to obtain JWT token from Hardwario Cloud.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<string?> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username cannot be empty.");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be empty.");

            try
            {
                _jwtToken = await GetAuthTokenAsync(_baseUrl, username, password);
                return _jwtToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot obtain JWT token from Hardwario Cloud. Error: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// This method is used to obtain JWT token from Hardwario Cloud v2.
        /// It is called automatically when using constructor with username and password.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> GetAuthTokenAsync(string baseUrl, string email, string password)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(baseUrl);

                var credentials = new { email = email, password = password };
                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(credentials), Encoding.UTF8, "application/json");

                HttpResponseMessage? response = null;
                try
                {
                    response = await httpClient.PostAsync("/v2/auth/login", content);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var token = System.Text.Json.JsonDocument.Parse(responseBody).RootElement.GetProperty("access_token").GetString();
                return token;
            }
        }

        private HttpClient GetHioClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new System.Uri(_baseUrl);

            if (_useapitoken)
                httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apitoken);
            else
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }

        private void CheckResponse(HttpResponseMessage? response)
        {
            if (response == null)
                throw new HttpRequestException("No response from server.");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                throw new Exception("Wrong API found. Page not found.");
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new Exception("Unauthorized access. Please check your credentials.");

            response.EnsureSuccessStatusCode();
        }

        #region Spaces

        /// <summary>
        /// Get list of all spaces
        /// You need to use JWT token version of the authorization because API tokens are created in specific spaces so they cannot see all spaces!
        /// </summary>
        /// <param name="num_of_items"></param>
        /// <param name="space_offset_id">space guid to offset from</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<HioCloudSpace>?> GetSpaces(int num_of_items = 100, string space_offset_id = "")
        {
            if (_useapitoken)
                throw new Exception("You cannot call all spaces with API token. Pleae initialize driver with use of JWT token or username and password.");

            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces?limit={num_of_items}";

                if (!string.IsNullOrEmpty(space_offset_id))
                { 
                    if (Guid.TryParse(space_offset_id, out var spaceguid))
                    { 
                        url = $"{url}&offset={space_offset_id}";
                    }
                }

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = response.Content.ReadAsStringAsync().Result;

                    CheckResponse(response);

                    var spaces = System.Text.Json.JsonSerializer.Deserialize<List<HioCloudSpace>>(cnt);
                    return spaces;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching spaces data.", ex);
                }
            }
        }

        /// <summary>
        /// Get specific space by ID
        /// </summary>
        /// <param name="space_id">GUID of space</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<HioCloudSpace?> GetSpace(Guid space_id)
        {
            if (space_id == Guid.Empty)
                throw new ArgumentException("Space ID cannot be empty.");

            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = response.Content.ReadAsStringAsync().Result;

                    CheckResponse(response);

                    var space = System.Text.Json.JsonSerializer.Deserialize<HioCloudSpace>(cnt);
                    return space;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching space data.", ex);
                }
            }
        }

        /// <summary>
        /// Create new space
        /// </summary>
        /// <param name="space_id"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<HioCloudSpace?> CreateSpace(Guid space_id, HioCloudSpace space)
        {
            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/";

                var content = new StringContent(JsonSerializer.Serialize(space), Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(url, content);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while adding user to customer. Response: {cnt}");
                    }
                    CheckResponse(response);

                    var spaceResponse = System.Text.Json.JsonSerializer.Deserialize<HioCloudSpace?>(cnt);
                    return spaceResponse;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching spaces data.", ex);
                }
            }
        }

        #endregion

        #region Devices

        /// <summary>
        /// Get all devices in space
        /// </summary>
        /// <param name="space_id">Guid of space</param>
        /// <param name="num_of_items">number of items to get</param>
        /// <param name="filter_logic">Filter logic all or any</param>
        /// <param name="sort_by">Sorting by</param>
        /// <param name="order_by">Order by</param>
        /// <param name="device_offset_id">Guid of device to do offset of listing</param>
        /// <param name="name">Name of device/s</param>
        /// <param name="serial_number">Filter by serial number of device</param>
        /// <param name="external_id">Filter by external id</param>
        /// <param name="tags_ids">Filter by tags list</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<List<HioCloudDevice>?> GetAllDevicesOfSpace(Guid space_id, 
                                                                       int num_of_items = 40, 
                                                                       string filter_logic = "any", 
                                                                       string sort_by = "id", 
                                                                       string order_by = "asc", 
                                                                       string device_offset_id = "", 
                                                                       string name = "", 
                                                                       string serial_number = "",
                                                                       string external_id = "",
                                                                       string[]? tags_ids = null)
        {
            
            if (!SortByFilters.IsSortByFilter(sort_by))
                throw new ArgumentException("Invalid sort_by value. Use one of the SortByFilters values.");

            if (!OrderByFilter.IsOrderByFilter(order_by))
                throw new ArgumentException("Invalid order_by value. Use one of the OrderByFilter values.");

            if (!FilterLogicFilters.IsFilterLogicFilter(filter_logic))
                throw new ArgumentException("Invalid filter_logic value. Use one of the FilterLogicFilters values.");

            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}/devices?limit={num_of_items}&sort_by={sort_by}&order_by={order_by}";

                if (tags_ids != null)
                {
                    url = $"{url}&tag_filter_logic={filter_logic}&tag_ids";
                    foreach (var tag in tags_ids)
                    {
                        if (Guid.TryParse(tag, out var tagguid))
                            url = $"{url}%5B%5D={tag}";
                        else
                            throw new ArgumentException("Invalid tag ID format. It must be form of valid GUID format.");
                    }
                }

                if (!string.IsNullOrEmpty(device_offset_id))
                {
                    if (Guid.TryParse(device_offset_id, out var deviceguid))
                    {
                        url = $"{url}&offset={device_offset_id}";
                    }
                }

                if (!string.IsNullOrEmpty(name))
                {
                    url = $"{url}&name={name}";
                }

                if (!string.IsNullOrEmpty(serial_number))
                {
                    url = $"{url}&serial_number={serial_number}";
                }

                if (!string.IsNullOrEmpty(external_id))
                {
                    if (Guid.TryParse(external_id, out var externalid))
                    {
                        url = $"{url}&external_id={external_id}";
                    }
                }

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = response.Content.ReadAsStringAsync().Result;

                    CheckResponse(response);

                    var devices = System.Text.Json.JsonSerializer.Deserialize<List<HioCloudDevice>?>(cnt);
                    return devices;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching spaces data.", ex);
                }
            }
        }

        /// <summary>
        /// Get device by ID
        /// </summary>
        /// <param name="space_id">Guid of space</param>
        /// <param name="device_id">Guid of device</param>
        /// <returns>Device</returns>
        /// <exception cref="Exception"></exception>
        public async Task<HioCloudDevice?> GetDevice(Guid space_id, Guid device_id)
        {

            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}/devices/{device_id}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = response.Content.ReadAsStringAsync().Result;

                    CheckResponse(response);

                    var device = System.Text.Json.JsonSerializer.Deserialize<HioCloudDevice?>(cnt);
                    return device;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching spaces data.", ex);
                }
            }
        }

        /// <summary>
        /// Create new device in cloud
        /// </summary>
        /// <param name="space_id"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<HioCloudDevice?> CreateDevice(Guid space_id, HioCloudDevice device)
        {

            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}/devices";

                var content = new StringContent(JsonSerializer.Serialize(device), Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(url, content);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while adding device. Response: {cnt}");
                    }
                    CheckResponse(response);

                    var deviceResponse = System.Text.Json.JsonSerializer.Deserialize<HioCloudDevice?>(cnt);
                    return deviceResponse;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while creating device.", ex);
                }
            }
        }

        /// <summary>
        /// Update device in cloud
        /// </summary>
        /// <param name="space_id"></param>
        /// <param name="device_id"></param>
        /// <param name="devupdate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<HioCloudDevice?> UpdateDevice(Guid space_id, Guid device_id, HioCloudDeviceUpdateRequest devupdate)
        {
            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}/devices/{device_id}";

                var content = new StringContent(JsonSerializer.Serialize(devupdate), Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PutAsync(url, content);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while adding tag. Response: {cnt}");
                    }
                    CheckResponse(response);

                    try
                    {
                        var dev = System.Text.Json.JsonSerializer.Deserialize<HioCloudDevice?>(cnt);
                        return dev;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Cannot deserialize data from the response to the HioCloudDevice class.", ex);
                    }
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while updating device.", ex);
                }
            }
        }

        /// <summary>
        /// Add Label to the device
        /// </summary>
        /// <param name="space_id"></param>
        /// <param name="device_id"></param>
        /// <param name="device"></param>
        /// <param name="labelname"></param>
        /// <param name="labelvalue"></param>
        /// <returns></returns>
        public async Task<HioCloudDevice?> AddDeviceLabel(Guid space_id, Guid device_id, HioCloudDevice device, string labelname, string labelvalue)
        {
            var devupdate = new HioCloudDeviceUpdateRequest()
            {
                Comment = device.Comment,
                Name = device.Name,
                Label = device.Label,
                Tags = device.Tags
            };

            if (devupdate.Label == null)
                devupdate.Label = new Dictionary<string, string>();

            devupdate.Label.Add(labelname, labelvalue);

            return await UpdateDevice(space_id, device_id, devupdate);
        }

        #endregion

            #region Messages

            /// <summary>
            /// Return All messages between two dates
            /// It solves paging because API offers max 100 messages at a time
            /// There is 500ms delay to prevent API rate limiting
            /// There is max messages count limit default to 5000
            /// </summary>
            /// <param name="space_id"></param>
            /// <param name="device_id"></param>
            /// <param name="after"></param>
            /// <param name="before"></param>
            /// <param name="delay"></param>
            /// <param name="maxmessages"></param>
            /// <returns></returns>
        public async Task <List<HioCloudMessage>> GetAllMessagesBetweenDates(Guid space_id, Guid device_id, DateTime after, DateTime before, int delay = 500, int maxmessages = 5000)
        {
            var allmessages = new List<HioCloudMessage>();
            var isLast = false;
            var lastmessageid = "";

            while (!isLast)
            {
                var messages = await GetAllDeviceMessages(space_id, device_id, 100, lastmessageid, after: after, before: before);
                if (messages != null && messages.Count > 0)
                {
                    allmessages.AddRange(messages);
                    lastmessageid = messages.Last().Id.ToString();
                }
                else
                {
                    isLast = true;
                    break;
                }
                if (allmessages.Count > maxmessages)
                    break;
                await Task.Delay(delay);
            }
            return allmessages;
        }

        /// <summary>
        /// Get all device messages
        /// If you need just one message use limit of num_of_items = 1
        /// </summary>
        /// <param name="space_id">Guid of space</param>
        /// <param name="device_id">Guid of device</param>
        /// <param name="num_of_items">number of messages to display</param>
        /// <param name="offset_message_id">Message Guid ID to create offset</param>
        /// <param name="direction">Direction filter</param>
        /// <param name="tag_id">Tag filter</param>
        /// <param name="after">After Date</param>
        /// <param name="before">Before Date</param>
        /// <param name="type">Type of message</param>
        /// <returns>Message List</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<List<HioCloudMessage>?> GetAllDeviceMessages(Guid space_id,
                                                                         Guid device_id,
                                                                         int num_of_items = 30,
                                                                         string offset_message_id = "",
                                                                         string direction = "",
                                                                         string tag_id = "",
                                                                         DateTime? after = null,
                                                                         DateTime? before = null,
                                                                         string[]? type = null,
                                                                         int timeout = 30)
        {

            if (!string.IsNullOrEmpty(direction) && !HioCloudMessageDirection.IsMessageDirection(direction))
                throw new ArgumentException("Invalid direction value. Use one of the HioCloudv2MessageDirection values.");
            
            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}/messages?device_id={device_id}&limit={num_of_items}";

                if (type != null)
                {
                    foreach (var t in type)
                    {
                        if (!string.IsNullOrEmpty(t) && !HioCloudMessageType.IsMessageType(t))
                            url = $"{url}%5B%5D={t}";
                    }
                }

                if (!string.IsNullOrEmpty(direction))
                {
                    url = $"{url}&direction={direction}";
                }

                if (!string.IsNullOrEmpty(tag_id))
                {
                    if (Guid.TryParse(tag_id, out var tid))
                    {
                        url = $"{url}&tag_id={tag_id}";
                    }
                }

                if (!string.IsNullOrEmpty(offset_message_id))
                {
                    if (Guid.TryParse(offset_message_id, out var oid))
                    {
                        url = $"{url}&offset={offset_message_id}";
                    }
                }

                if (after != null)
                {
                    url = $"{url}&after={after.Value.ToString("yyyy-MM-ddTHHaaa3Ammaaa3Ass").Replace("aaa", "%")}";
                }
                else
                {
                    url = $"{url}&after=2016-01-01T08:00:00.000Z";
                }

                if (before != null)
                {
                    url = $"{url}&before={before.Value.ToString("yyyy-MM-ddTHHaaa3Ammaaa3Ass").Replace("aaa", "%")}";
                }

                try
                {
                    httpClient.Timeout = new TimeSpan(0,0,timeout);
                    var response = await httpClient.GetAsync(url);
                    var cnt = response.Content.ReadAsStringAsync().Result;

                    CheckResponse(response);

                    var devices = System.Text.Json.JsonSerializer.Deserialize<List<HioCloudMessage>?>(cnt);
                    return devices;
                }
                catch (HttpRequestException ex)
                {
                    //throw new Exception("An error occurred while fetching spaces data.", ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Get one specific message
        /// </summary>
        /// <param name="space_id">Guid of space</param>
        /// <param name="message_id">Guid of message</param>
        /// <returns>Message List</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<HioCloudMessage?> GetMessage(Guid space_id,
                                                       Guid message_id)
        {
            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}/messages/{message_id}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = response.Content.ReadAsStringAsync().Result;

                    CheckResponse(response);

                    var devices = System.Text.Json.JsonSerializer.Deserialize<HioCloudMessage?>(cnt);
                    return devices;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching spaces data.", ex);
                }
            }
        }

        /// <summary>
        /// Send downlink message to cloud to the specific device
        /// This version will send message_body as string
        /// </summary>
        /// <param name="space_id">Guid of space</param>
        /// <param name="device_id">Guid of device</param>
        /// <param name="message_body">Body to send as string</param>
        /// <param name="message_type">Use HioCloudv2MessageType static class to get types</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<HioCloudMessage?> AddNewDownlingMessage(Guid space_id,
                                                                    Guid device_id,
                                                                    string message_body,
                                                                    string message_type)
        {

            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}/messages";

                try
                {
                    var body = new 
                    { 
                        device_id = device_id, 
                        body = message_body,
                        type = message_type
                    };

                    var response = await httpClient.PostAsync(url, 
                                                              new StringContent(System.Text.Json.JsonSerializer.Serialize(body), 
                                                              Encoding.UTF8, "application/json"));

                    var cnt = response.Content.ReadAsStringAsync().Result;

                    CheckResponse(response);

                    var msg = System.Text.Json.JsonSerializer.Deserialize<HioCloudMessage?>(cnt);
                    return msg;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching spaces data.", ex);
                }
            }
        }

        /// <summary>
        /// Send downlink message to cloud to the specific device
        /// This version will send the message_body as object
        /// </summary>
        /// <param name="space_id">Guid of space</param>
        /// <param name="device_id">Guid of device</param>
        /// <param name="message_body">Body to send as object</param>
        /// <param name="message_type">Use HioCloudv2MessageType static class to get types</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<HioCloudMessage?> AddNewDownlinkMessage(Guid space_id,
                                                                    Guid device_id,
                                                                    object message_body,
                                                                    string message_type)
        {

            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}/messages";

                try
                {
                    var body = new 
                    { 
                        device_id = device_id, 
                        body = message_body,
                        type = message_type
                    };

                    var bodystr = System.Text.Json.JsonSerializer.Serialize(body);

                    var response = await httpClient.PostAsync(url, 
                                                              new StringContent(bodystr, 
                                                              Encoding.UTF8, "application/json"));

                    var cnt = response.Content.ReadAsStringAsync().Result;

                    CheckResponse(response);

                    var msg = System.Text.Json.JsonSerializer.Deserialize<HioCloudMessage?>(cnt);
                    return msg;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching spaces data.", ex);
                }
            }
        }

        #endregion

        #region Tags

        /// <summary>
        /// Create new tag
        /// </summary>
        /// <param name="space_id"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<HioCloudTag?> CreateTag(Guid space_id, HioCloudTag tag)
        {
            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}/tags";

                var content = new StringContent(JsonSerializer.Serialize(tag), Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(url, content);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while adding tag. Response: {cnt}");
                    }
                    CheckResponse(response);

                    var tagResponse = System.Text.Json.JsonSerializer.Deserialize<HioCloudTag?>(cnt);
                    return tagResponse;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while creating tag.", ex);
                }
            }
        }

        /// <summary>
        /// Get all tags in space
        /// </summary>
        /// <param name="space_id">Guid of space</param>
        /// <returns>Device</returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<HioCloudTag>?> GetTags(Guid space_id, int num_of_items = 100)
        {

            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}/tags?limit={num_of_items}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = response.Content.ReadAsStringAsync().Result;

                    CheckResponse(response);

                    var device = System.Text.Json.JsonSerializer.Deserialize<List<HioCloudTag>?>(cnt);
                    return device;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while getting tags in space.", ex);
                }
            }
        }

        /// <summary>
        /// Get all tags in space
        /// </summary>
        /// <param name="space_id">Guid of space</param>
        /// <returns>Device</returns>
        /// <exception cref="Exception"></exception>
        public async Task<HioCloudTag?> GetTag(Guid space_id, Guid tag_id)
        {

            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}/tags{tag_id}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = response.Content.ReadAsStringAsync().Result;

                    CheckResponse(response);

                    var device = System.Text.Json.JsonSerializer.Deserialize<HioCloudTag?>(cnt);
                    return device;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while getting tag.", ex);
                }
            }
        }

        /// <summary>
        /// Add existing tag to exiting device
        /// </summary>
        /// <param name="space_id"></param>
        /// <param name="device_id"></param>
        /// <param name="tag"></param>
        /// <param name="devupdate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AddTagToDevice(Guid space_id, HioCloudTag tag, HioCloudDevice? device)
        {
            if (device == null)
                throw new ArgumentException("AddTagToDevice>>Device cannot be null.");
            try
            {
                if (!device.Tags.Any(t => t.Id == tag.Id))
                {
                    device.Tags.Add(tag);
                }

                var devupdate = new HioCloudDeviceUpdateRequest()
                {
                    Comment = device.Comment,
                    Name = device.Name,
                    Label = device.Label,
                    Tags = device.Tags
                };

                var dev = await UpdateDevice(space_id,(Guid)device.Id, devupdate);
                if (dev != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("An error occurred while adding tag to device.", ex);
            }
        }

        #endregion

        #region Connectors

        /// <summary>
        /// Get All connectors in the space
        /// </summary>
        /// <param name="space_id"></param>
        /// <param name="num_of_items"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<HioCloudConnector>?> GetConnectors(Guid space_id, int num_of_items = 100)
        {

            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}/connectors?limit={num_of_items}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = response.Content.ReadAsStringAsync().Result;

                    CheckResponse(response);

                    try
                    {
                        var device = System.Text.Json.JsonSerializer.Deserialize<List<HioCloudConnector>?>(cnt);
                        return device;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Cannot deserialize data from the response to the HioCloudConnector class.", ex);
                        return null;
                    }
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while getting tags in space.", ex);
                }
            }
        }

        /// <summary>
        /// Get one specific connector
        /// </summary>
        /// <param name="space_id"></param>
        /// <param name="connector_id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<HioCloudConnector?> GetConnector(Guid space_id, Guid connector_id)
        {

            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}/connectors/{connector_id}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = response.Content.ReadAsStringAsync().Result;

                    CheckResponse(response);

                    try
                    {
                        var device = System.Text.Json.JsonSerializer.Deserialize<HioCloudConnector?>(cnt);
                        return device;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Cannot deserialize data from the response to the HioCloudConnector class.", ex);
                        return null;
                    }
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while getting tags in space.", ex);
                }
            }
        }

        /// <summary>
        /// Add new connector
        /// </summary>
        /// <param name="space_id"></param>
        /// <param name="connector"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<HioCloudConnector?> CreateConnector(Guid space_id, HioCloudConnector connector)
        {
            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}/connectors";

                var content = new StringContent(JsonSerializer.Serialize(connector), Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(url, content);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while adding connector. Response: {cnt}");
                    }
                    CheckResponse(response);

                    var connectorResponse = System.Text.Json.JsonSerializer.Deserialize<HioCloudConnector?>(cnt);
                    return connectorResponse;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching spaces data.", ex);
                }
            }
        }

        /// <summary>
        /// Update already existing connector
        /// </summary>
        /// <param name="space_id"></param>
        /// <param name="connector"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<HioCloudConnector?> UpdateConnector(Guid space_id, HioCloudConnector connector)
        {
            using (var httpClient = GetHioClient())
            {
                var url = $"/v2/spaces/{space_id}/connectors";

                var content = new StringContent(JsonSerializer.Serialize(connector), Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PutAsync(url, content);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while adding connector. Response: {cnt}");
                    }
                    CheckResponse(response);

                    var connectorResponse = System.Text.Json.JsonSerializer.Deserialize<HioCloudConnector?>(cnt);
                    return connectorResponse;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching spaces data.", ex);
                }
            }
        }

        #endregion
    }


}

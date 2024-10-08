﻿using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using hio_dotnet.APIs.ThingsBoard.Models;
using hio_dotnet.Common.Helpers;
using hio_dotnet.APIs.ThingsBoard.Models.Dashboards;

namespace hio_dotnet.APIs.ThingsBoard
{
    public class ThingsBoardDriver
    {
        public static string DefaultHardwarioThingsboardUrl = "https://thingsboard.hardwario.com";

        /// <summary>
        /// Use this constructor to authenticate with ThingsBoard using username and password.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="port"></param>
        /// <exception cref="ArgumentException"></exception>
        public ThingsBoardDriver(string baseUrl, string username, string password, int port = 0)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("Base URL cannot be empty.");
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username cannot be empty.");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be empty.");

            _baseUrl = port > 0 ? $"{baseUrl}:{port}" : baseUrl;
            _port = port;
            _jwtToken = GetAuthTokenAsync(_baseUrl, username, password).Result;
        }

        /// <summary>
        /// Use this constructor to authenticate with ThingsBoard using JWT token.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="jwtToken"></param>
        /// <param name="port"></param>
        /// <exception cref="ArgumentException"></exception>
        public ThingsBoardDriver(string baseUrl, string jwtToken, int port = 0)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("Base URL cannot be empty.");
            if (string.IsNullOrEmpty(jwtToken))
                throw new ArgumentException("JWT token cannot be empty.");

            _baseUrl = baseUrl;
            _port = port;
            _jwtToken = jwtToken;
        }

        private string _baseUrl = string.Empty;
        private int _port = 0;
        private string _jwtToken = string.Empty;


        #region Telemetry

        /// <summary>
        /// Parsing of the data received from ThingsBoard for keys in telemetry response.
        /// </summary>
        /// <param name="jsonResponse"></param>
        /// <returns>Dictionary of key name and value without timestamp data</returns>
        /// <exception cref="Exception"></exception>
        public async Task<Dictionary<string, double>> ParseActualTelemetryJustValueDataAsync(string jsonResponse)
        {
            try
            {
                var jsonDocument = JsonDocument.Parse(jsonResponse);
                var telemetryData = new Dictionary<string, double>();

                foreach (var property in jsonDocument.RootElement.EnumerateObject())
                {
                    var key = property.Name;
                    var valueArray = property.Value.EnumerateArray().FirstOrDefault();

                    if (valueArray.ValueKind == JsonValueKind.Object)
                    {
                        var valueString = valueArray.GetProperty("value").GetString();
                        if (double.TryParse(valueString, CultureInfo.InvariantCulture, out var valueDouble))
                        {
                            telemetryData[key] = valueDouble;
                        }
                    }
                }

                return telemetryData;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while parsing telemetry data.", ex);
            }
        }

        /// <summary>
        /// Parsing of the data received from ThingsBoard for keys in telemetry response.
        /// </summary>
        /// <param name="jsonResponse"></param>
        /// <returns>Dictionary with key and DataValue object which includes timestamp and value</returns>
        /// <exception cref="Exception"></exception>
        public async Task<Dictionary<string, DataValue>> ParseActualTelemetryDataAsync(string jsonResponse)
        {
            try
            {
                var jsonDocument = JsonDocument.Parse(jsonResponse);
                var telemetryData = new Dictionary<string, DataValue>();

                foreach (var property in jsonDocument.RootElement.EnumerateObject())
                {
                    var key = property.Name;
                    var valueArray = property.Value.EnumerateArray().FirstOrDefault();

                    if (valueArray.ValueKind == JsonValueKind.Object)
                    {
                        var valueString = valueArray.GetProperty("value").GetString();
                        var ts = valueArray.GetProperty("ts").GetUInt64();

                        if (double.TryParse(valueString, CultureInfo.InvariantCulture, out double valueDouble))
                        {
                            telemetryData[key] = new DataValue { Timestamp = TimeHelpers.UnixTimestampToDateTime(ts), Value = valueDouble };
                        }
                    }
                }

                return telemetryData;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while parsing telemetry data.", ex);
            }
        }

        /// <summary>
        /// Parsing of the historic data received from ThingsBoard for keys in telemetry response.
        /// </summary>
        /// <param name="jsonResponse"></param>
        /// <returns>Dictionary with key and DataValue object which includes timestamp and value</returns>
        /// <exception cref="Exception"></exception>
        public async Task<Dictionary<string, List<DataValue>>> ParseHistoricTelemetryDataAsync(string jsonResponse)
        {
            try
            {
                var jsonDocument = JsonDocument.Parse(jsonResponse);
                var telemetryData = new Dictionary<string, List<DataValue>>();

                foreach (var property in jsonDocument.RootElement.EnumerateObject())
                {
                    var key = property.Name;
                    telemetryData[key] = new List<DataValue>();

                    foreach (var valueArray in property.Value.EnumerateArray())
                    {
                        if (valueArray.ValueKind == JsonValueKind.Object)
                        {
                            var valueString = valueArray.GetProperty("value").GetString();
                            var ts = valueArray.GetProperty("ts").GetUInt64();

                            if (double.TryParse(valueString, CultureInfo.InvariantCulture, out double valueDouble))
                            {
                                telemetryData[key].Add(new DataValue
                                {
                                    Timestamp = TimeHelpers.UnixTimestampToDateTime(ts),
                                    Value = valueDouble
                                });
                            }
                        }
                    }
                }

                return telemetryData;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while parsing telemetry data.", ex);
            }
        }

        /// <summary>
        /// Get telemetry data from ThingsBoard.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="deviceId"></param>
        /// <param name="keys">specify the keys you want to obtain. If multiple keys are required use ',' as separator</param>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<string> GetTelemetryDataAsync(string deviceId, string keys, string jwtToken = "")
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                jwtToken = _jwtToken;
            }
            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new ArgumentException("JWT token cannot be empty. If you do not know JWT token use constructor with Username and Password to obtain JWT token automatically.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = $"/api/plugins/telemetry/DEVICE/{deviceId}/values/timeseries?keys={keys}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching telemetry data.", ex);
                }
            }
        }

        /// <summary>
        /// Get telemetry data from ThingsBoard for specified keys within a specific time range.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="deviceId"></param>
        /// <param name="keys">Specify the keys you want to obtain. If multiple keys are required, use ',' as separator</param>
        /// <param name="start">Start time of the time range</param>
        /// <param name="end">End time of the time range</param>
        /// <param name="jwtToken"></param>
        /// <returns>Telemetry data as a JSON string</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<string> GetTelemetryDataForTimeRangeAsync(string deviceId, string keys, DateTime start, DateTime end, string jwtToken = "")
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                jwtToken = _jwtToken;
            }
            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new ArgumentException("JWT token cannot be empty. If you do not know JWT token use constructor with Username and Password to obtain JWT token automatically.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Convert DateTime to Unix timestamp in milliseconds
                long startTimestamp = new DateTimeOffset(start).ToUnixTimeMilliseconds();
                long endTimestamp = new DateTimeOffset(end).ToUnixTimeMilliseconds();

                var url = $"/api/plugins/telemetry/DEVICE/{deviceId}/values/timeseries?keys={keys}&startTs={startTimestamp}&endTs={endTimestamp}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching telemetry data for the specified time range.", ex);
                }
            }
        }

        public async Task<string?> SendTelemetryData(object telemetryData, string connectionToken, string jwtToken = "")
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                jwtToken = _jwtToken;
            }
            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new ArgumentException("JWT token cannot be empty. If you do not know JWT token use constructor with Username and Password to obtain JWT token automatically.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var data = JsonSerializer.Serialize(telemetryData);

                var url = $"/api/v1/{connectionToken}/telemetry";
                var content = new StringContent(data, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(url, content);
                    var cnt = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while creating a customer.", ex);
                }
            }
        }

        #endregion

        #region Attributes
        /// <summary>
        /// Get attributes from ThingsBoard.
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <param name="keys">Comma-separated keys to obtain</param>
        /// <param name="jwtToken">JWT Token (optional)</param>
        /// <returns>Attributes as a JSON string</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<string> GetAttributesAsync(string deviceId, string keys, string jwtToken = "")
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                jwtToken = _jwtToken;
            }
            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new ArgumentException("JWT token cannot be empty. If you do not know JWT token use constructor with Username and Password to obtain JWT token automatically.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = $"/api/plugins/telemetry/DEVICE/{deviceId}/attributes?keys={keys}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching attributes.", ex);
                }
            }
        }

        /// <summary>
        /// Set attributes in ThingsBoard.
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <param name="attributes">Attributes in key-value format as JSON</param>
        /// <param name="jwtToken">JWT Token (optional)</param>
        /// <returns>Response from the ThingsBoard API</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<string> SetAttributesAsync(string deviceId, Dictionary<string, object> attributes, string jwtToken = "")
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                jwtToken = _jwtToken;
            }
            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new ArgumentException("JWT token cannot be empty. If you do not know JWT token use constructor with Username and Password to obtain JWT token automatically.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = $"/api/plugins/telemetry/DEVICE/{deviceId}/attributes";
                var content = new StringContent(JsonSerializer.Serialize(attributes), Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while setting attributes.", ex);
                }
            }
        }

        #endregion

        #region Customers

        /// <summary>
        /// Create a new customer in ThingsBoard.
        /// </summary>
        /// <param name="customer">Customer object containing details</param>
        /// <param name="jwtToken">JWT Token (optional)</param>
        /// <returns>Response from the ThingsBoard API</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<Customer?> CreateCustomerAsync(CreateCustomerRequest customer, string jwtToken = "")
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                jwtToken = _jwtToken;
            }
            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new ArgumentException("JWT token cannot be empty. If you do not know JWT token use constructor with Username and Password to obtain JWT token automatically.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = "/api/customer";
                var content = new StringContent(JsonSerializer.Serialize(customer), Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(url, content);
                    var cnt = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Customer>(responseBody);
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while creating a customer.", ex);
                }
            }
        }

        //Delete customer request
        public async Task<string> DeleteCustomerAsync(string customerId, string jwtToken = "")
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                jwtToken = _jwtToken;
            }
            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new ArgumentException("JWT token cannot be empty. If you do not know JWT token use constructor with Username and Password to obtain JWT token automatically.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = $"/api/customer/{customerId}";

                try
                {
                    var response = await httpClient.DeleteAsync(url);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while deleting customer.", ex);
                }
            }
        }

        /// <summary>
        /// Get customer details from ThingsBoard by customer ID.
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="jwtToken">JWT Token (optional)</param>
        /// <returns>Customer object containing details</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<Customer?> GetCustomerByIdAsync(string customerId, string jwtToken = "")
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                jwtToken = _jwtToken;
            }
            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new ArgumentException("JWT token cannot be empty. If you do not know JWT token use constructor with Username and Password to obtain JWT token automatically.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = $"/api/customer/{customerId}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Customer>(responseBody);
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching customer details.", ex);
                }
            }
        }


        #endregion

        #region DeviceProfiles
        /// <summary>
        /// Get device profiles from ThingsBoard.
        /// </summary>
        /// <param name="jwtToken">JWT Token (optional)</param>
        /// <returns>List of DeviceProfile objects</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<List<DeviceProfile>?> GetDeviceProfilesAsync(string jwtToken = "", int pageSize = 10, int page = 0)
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                jwtToken = _jwtToken;
            }
            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new ArgumentException("JWT token cannot be empty. If you do not know JWT token use constructor with Username and Password to obtain JWT token automatically.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = $"/api/deviceProfiles?pageSize={pageSize}&page={page}&sortProperty=createdTime&sortOrder=ASC";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var deviceProfileResponse = JsonSerializer.Deserialize<DeviceProfilesResponse>(responseBody);
                    return deviceProfileResponse?.Data ?? null;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching device profiles.", ex);
                }
            }
        }

        #endregion

        #region Devices
        /// <summary>
        /// Create a new device in ThingsBoard.
        /// </summary>
        /// <param name="device">Device object containing details</param>
        /// <param name="jwtToken">JWT Token (optional)</param>
        /// <returns>Response from the ThingsBoard API</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<Device?> CreateDeviceAsync(CreateDeviceRequest device, string jwtToken = "")
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                jwtToken = _jwtToken;
            }
            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new ArgumentException("JWT token cannot be empty. If you do not know JWT token use constructor with Username and Password to obtain JWT token automatically.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = "/api/device";
                var content = new StringContent(JsonSerializer.Serialize(device), Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(url, content);
                    var cnt = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Device>(responseBody);
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while creating a customer.", ex);
                }
            }
        }

        /// <summary>
        /// Get device connection parameters info
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <param name="jwtToken">JWT Token (optional)</param>
        /// <returns>List of DeviceProfile objects</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<DeviceConnectivity?> GetDeviceConnectionInfoAsync(string deviceId, string jwtToken = "")
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                jwtToken = _jwtToken;
            }
            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new ArgumentException("JWT token cannot be empty. If you do not know JWT token use constructor with Username and Password to obtain JWT token automatically.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = $"/api/device-connectivity/{deviceId}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var deviceConnectivityInfoResponse = JsonSerializer.Deserialize<DeviceConnectivity>(responseBody);
                    return deviceConnectivityInfoResponse ?? null;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching device profiles.", ex);
                }
            }
        }

        public string? ParseConnectionToken(DeviceConnectivity connectivity)
        {
            if (connectivity != null)
            {
                if (connectivity.Http != null && connectivity.Http.Http != null)
                {
                    // parse from string: "curl -v -X POST http://localhost:8080/api/v1/7U8BwB6UzhWXmXgV0i74/telemetry --header Content-Type:application/json --data "{temperature:25}""
                    var split = connectivity.Http.Http.Split("/api/v1/");
                    if (split.Length > 0)
                    {
                        var token = split[1].Split("/telemetry");
                        if (token.Length > 0)
                        {
                            return token[0];
                        }
                    }
                }
            }

            return null;
        }

        //Delete device request
        public async Task<string> DeleteDeviceAsync(string deviceId, string jwtToken = "")
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                jwtToken = _jwtToken;
            }
            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new ArgumentException("JWT token cannot be empty. If you do not know JWT token use constructor with Username and Password to obtain JWT token automatically.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = $"/api/device/{deviceId}";

                try
                {
                    var response = await httpClient.DeleteAsync(url);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while deleting device.", ex);
                }
            }
        }

        #endregion

        #region Dashboards

        /// <summary>
        /// Create a new dashboard in ThingsBoard.
        /// </summary>
        /// <param name="dashboardRequest">Dashboard object containing details</param>
        /// <param name="jwtToken">JWT Token (optional)</param>
        /// <returns>Response from the ThingsBoard API</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<Dashboard?> CreateDashboardAsync(CreateDashboardRequest dashboardRequest, string jwtToken = "")
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                jwtToken = _jwtToken;
            }
            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new ArgumentException("JWT token cannot be empty. If you do not know JWT token use constructor with Username and Password to obtain JWT token automatically.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var data = JsonSerializer.Serialize(dashboardRequest);

                var url = "/api/dashboard";
                var content = new StringContent(data, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync(url, content);
                    var cnt = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Dashboard>(responseBody);
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while creating a customer.", ex);
                }
            }
        }

        //Delete dashboard request
        public async Task<string> DeleteDashboardAsync(string dashboardId, string jwtToken = "")
        {
            if (string.IsNullOrEmpty(jwtToken))
            {
                jwtToken = _jwtToken;
            }
            if (string.IsNullOrEmpty(jwtToken))
            {
                throw new ArgumentException("JWT token cannot be empty. If you do not know JWT token use constructor with Username and Password to obtain JWT token automatically.");
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = $"/api/dashboard/{dashboardId}";

                try
                {
                    var response = await httpClient.DeleteAsync(url);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while deleting dashboard.", ex);
                }
            }
        }

        #endregion

        /// <summary>
        /// This method is used to obtain JWT token from ThingsBoard.
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

                var credentials = new { username = email, password = password };
                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(credentials), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("/api/auth/login", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var token = System.Text.Json.JsonDocument.Parse(responseBody).RootElement.GetProperty("token").GetString();
                return token;
            }
        }


    }
}

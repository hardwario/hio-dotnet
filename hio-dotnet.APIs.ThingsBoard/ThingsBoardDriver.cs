using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using hio_dotnet.APIs.ThingsBoard.Models;
using hio_dotnet.Common.Helpers;

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

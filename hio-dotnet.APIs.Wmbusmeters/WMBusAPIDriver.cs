﻿using hio_dotnet.APIs.Wmbusmeters.Models;
using System.Net.Http.Headers;

namespace hio_dotnet.APIs.Wmbusmeters
{
    public class WMBusAPIDriver
    {
        public static string DefaultHardwarioThingsboardUrl = "https://thingsboard.hardwario.com/wmbusmeters/";

        /// <summary>
        /// Use this constructor to authenticate with ThingsBoard using username and password.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="port"></param>
        /// <exception cref="ArgumentException"></exception>
        public WMBusAPIDriver(string baseUrl = "", int port = 0)
        {
            if (string.IsNullOrEmpty(baseUrl))
                baseUrl = DefaultHardwarioThingsboardUrl;

            _baseUrl = port > 0 ? $"{baseUrl}:{port}" : baseUrl;
            _port = port;
        }

        private string _baseUrl = string.Empty;
        private int _port = 0;

        private HttpClient GetHioClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new System.Uri(_baseUrl);

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

        /// <summary>
        /// Analyze telegram from WM-Bus meter with optional drivername and password.
        /// </summary>
        /// <param name="telegram">WM-Bus meter telegram</param>
        /// <param name="drivername">Optional, keep as auto if you do not know the type of driver</param>
        /// <param name="password">Optional</param>
        /// <returns>Tuple of original string response and, parsed object based on the type of the WMBusMeter</returns>
        /// <exception cref="Exception"></exception>
        public async Task<Tuple<string,object?>> AnalyzeTelegram(string telegram, string drivername = "auto", string password = "")
        {

            using (var httpClient = GetHioClient())
            {
                var url = $"analyze/{telegram}";


                if (!string.IsNullOrEmpty(password))
                {
                        url = $"{url}:{drivername}:{password}";
                }

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = response.Content.ReadAsStringAsync().Result;

                    CheckResponse(response);

                    var common = System.Text.Json.JsonSerializer.Deserialize<WMBusMetersCommon>(cnt);
                    if (common != null)
                    {
                        if (common.Media == "electricity")
                        {
                            return new Tuple<string,object>(cnt, System.Text.Json.JsonSerializer.Deserialize<WMBusMetersElectricityBase>(cnt) ?? null);
                        }
                        else if (common.Media == "gas")
                        {
                            return new Tuple<string, object>(cnt, System.Text.Json.JsonSerializer.Deserialize<WMBusMetersGasBase>(cnt) ?? null);
                        }
                        else if (common.Media == "heat cost allocation" || common.Media == "heat cost allocator")
                        {
                            return new Tuple<string, object>(cnt, System.Text.Json.JsonSerializer.Deserialize<WMBusMetersHcaBase>(cnt) ?? null);
                        }
                        else if (common.Media == "water")
                        {
                            return new Tuple<string, object>(cnt, System.Text.Json.JsonSerializer.Deserialize<WMBusMetersWaterBase>(cnt) ?? null);
                        }
                    }
                    
                    return new Tuple<string, object>(cnt, common ?? null);
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while fetching spaces data.", ex);
                }
            }
        }
    }
}
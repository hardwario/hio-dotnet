﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.Server
{
    public class DriversServerApiClient
    {
        public static string DefaultUrl = "http://localhost:8042";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="port"></param>
        /// <exception cref="ArgumentException"></exception>
        public DriversServerApiClient(string? baseUrl, int port = 0)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                _baseUrl = DefaultUrl;
            }
            else
            {
                _baseUrl = port > 0 ? $"{baseUrl}:{port}" : baseUrl;
                _port = port;
            }
        }

        private string _baseUrl = string.Empty;
        private int _port = 0;

        /// <summary>
        /// Get PPK2 ports names
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<GetPortsResponse>> PPK2_GetPortsNames()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);

                var url = $"/api/ppk2/getportsnames";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while getting port names. Response: {cnt}");
                    }
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<GetPortsResponse>> (responseBody) ?? null;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while getting port names.", ex);
                }
            }
        }

        /// <summary>
        /// Initialize PPK2 on specific port
        /// </summary>
        /// <param name="portname"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> PPK2_Init(string portname)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                portname = System.Web.HttpUtility.UrlEncode(portname);
                var url = $"/api/ppk2/init/{portname}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while initing ppk2. Response: {cnt}");
                    }
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while initing ppk2.", ex);
                }
            }
        }

        /// <summary>
        /// Set voltage for PPK2
        /// </summary>
        /// <param name="voltage">in millivolts</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> PPK2_SetVoltage(int voltage)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);

                var url = $"/api/ppk2/setvoltage/{voltage}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while getting setting voltage. Response: {cnt}");
                    }
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while setting voltage.", ex);
                }
            }
        }

        /// <summary>
        /// Turn on PPK2
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> PPK2_TurnOn()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);

                var url = $"/api/ppk2/turnon";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while getting turning on ppk2. Response: {cnt}");
                    }
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while turning on ppk2.", ex);
                }
            }
        }

        /// <summary>
        /// Turn off PPK2
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> PPK2_TurnOff()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);

                var url = $"/api/ppk2/turnoff";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while getting turning off ppk2. Response: {cnt}");
                    }
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while turning off ppk2.", ex);
                }
            }
        }

        /// <summary>
        /// PPK2: Get Device Status
        /// If true, device is on
        /// If false, device is off
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> PPK2_DeviceStatus()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);

                var url = $"/api/ppk2/devicestatus";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while getting ppk2 device status. Response: {cnt}");
                    }
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<bool>(responseBody);
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while ppk2 device status.", ex);
                }
            }
        }

        /// <summary>
        /// PPK2: Get Device Voltage
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<int> PPK2_DeviceVoltage()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);

                var url = $"/api/ppk2/devicevoltage";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while getting ppk2 device voltage. Response: {cnt}");
                    }
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<int>(responseBody);
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while ppk2 device voltage.", ex);
                }
            }
        }

        /// <summary>
        /// Initialize JLink
        /// </summary>
        /// <param name="withPPK2"></param>
        /// <param name="mcu"></param>
        /// <param name="speed"></param>
        /// <param name="rttaddr"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> JLink_Init(bool withPPK2 = true, string mcu = "nRF52840_xxAA", int speed = 4000, uint rttaddr = 0)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);

                var url = $"/api/jlink/init/{withPPK2}/{mcu}/{speed}/{rttaddr}";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while initialization of jlink. Response: {cnt}");
                    }
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while initialization of jlink.", ex);
                }
            }
        }

        /// <summary>
        /// Stop JLink
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> JLink_Stop()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);

                var url = $"/api/jlink/stop";

                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while stopping of jlink. Response: {cnt}");
                    }
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while stopping of jlink.", ex);
                }
            }
        }

        /// <summary>
        /// Send command by channel to JLink
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> JLink_SendCommandByChannel(int channel, string message)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);

                // encode message to be save as part of the url
                message = System.Web.HttpUtility.UrlEncode(message);

                var url = $"/api/jlink/sendcommandbychannel/{channel}/{message}";
                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while sending command by channel to jlink. Response: {cnt}");
                    }
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while sending command by channel to jlink.", ex);
                }
            }
        }

        /// <summary>
        /// Send command by name to JLink
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> JLink_SendCommandByName(string name, string message)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);
                message = System.Web.HttpUtility.UrlEncode(message);
                var url = $"/api/jlink/sendcommandbyname/{name}/{message}";
                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while sending command by name to jlink. Response: {cnt}");
                    }
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while sending command by name to jlink.", ex);
                }
            }
        }

        /// <summary>
        /// Load all commands from help
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> JLink_LoadAllCommandsFromHelp(int channel, string parent)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(_baseUrl);

                // encode message to be save as part of the url
                parent = System.Web.HttpUtility.UrlEncode(parent);

                var url = $"/api/jlink/loadcommandsfromhelp/{channel}";
                if (!string.IsNullOrEmpty(parent))
                {
                    url += $"/{parent}";
                }
                try
                {
                    var response = await httpClient.GetAsync(url);
                    var cnt = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred while sending command by channel to jlink. Response: {cnt}");
                    }
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("An error occurred while sending command by channel to jlink.", ex);
                }
            }
        }
    }
}

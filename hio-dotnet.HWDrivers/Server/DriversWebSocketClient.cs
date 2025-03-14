using hio_dotnet.HWDrivers.MCU;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.Server
{
    public class DriversWebSocketClient
    {
        private ClientWebSocket _webSocket;

        public event Action<string> OnMessageReceived;

        public ConcurrentDictionary<Guid, DriversWebSocketRequest> Requests { get; set; } = new ConcurrentDictionary<Guid, DriversWebSocketRequest>();
        public ConcurrentDictionary<Guid, DriversWebSocketResponse> Responses { get; set; } = new ConcurrentDictionary<Guid, DriversWebSocketResponse>();
        public async Task ConnectAsync(string uri)
        {
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
            Console.WriteLine("Connected to WebSocket server");

            await SendMessageAsync("Connected to WebSocket server");
            await ReceiveMessagesAsync();
        }

        public async Task SendMessageAsync(string message)
        {
            if (_webSocket?.State == WebSocketState.Open)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024 * 4];
            while (_webSocket?.State == WebSocketState.Open)
            {
                try
                {
                    await Task.Delay(5);
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    //Console.WriteLine($"New message captured:{message}");
                    if (message != null)
                    {
                        if (message.Contains("WSSessionMessage:"))
                        {
                            var parts = message.Split("WSSessionMessage:");
                            if (parts.Length == 2)
                            {
                                message = parts[1];
                            }
                        }

                        if (message.Contains("DriversWebSocketModuleApiResponse:"))
                        {
                            var parts = message.Split("DriversWebSocketModuleApiResponse:");
                            if (parts.Length == 2)
                            {
                                var apiResponse = parts[1];
                                var response = JsonSerializer.Deserialize<DriversWebSocketResponse>(apiResponse);
                                if (response != null)
                                {
                                    if (Responses.ContainsKey(response.Id))
                                        Responses.TryRemove(response.Id, out var r);
                                    Responses.TryAdd(response.Id, response);
                                }
                            }
                        }
                        OnMessageReceived?.Invoke(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving message: {ex.Message}");
                }
            }
        }

        public async Task SendMessage(MultiRTTClientWSMessage message)
        {
            if (_webSocket?.State == WebSocketState.Open)
            {
                var json = System.Text.Json.JsonSerializer.Serialize(message);
                var bytes = Encoding.UTF8.GetBytes(json);
                await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public async Task DisconnectAsync()
        {
            if (_webSocket != null)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                _webSocket.Dispose();
                _webSocket = null;
            }
        }

        /// <summary>
        /// Send API Request to Drivers Server and wait for the response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="timeout">base delay is 100ms. for example 30s is timeout=300</param>
        /// <returns></returns>
        public async Task<string> SendApiRequest(DriversWebSocketRequest request, int timeout = 300)
        {
            var json = JsonSerializer.Serialize(request);
            Requests.TryAdd(request.Id, request);
            await SendMessageAsync($"DriversWebSocketModuleApiRequest:{json}");

            var timeoutCounter = 0;
            var foundResponse = false;
            while (timeoutCounter < timeout)
            {
                await Task.Delay(100);
                if (Responses.ContainsKey(request.Id))
                {
                    foundResponse = true;
                    break;
                }
                timeoutCounter++;
            }

            if (foundResponse)
            {
                var response = Responses[request.Id];
                Responses.TryRemove(request.Id, out var r);
                return response.Response;
            }

            return string.Empty;
        }

        /// <summary>
        /// Get PPK2 Ports Names
        /// </summary>
        /// <returns></returns>
        public async Task<List<GetPortsResponse>> PPK2_GetPortsNames()
        {
            var request = new DriversWebSocketRequest()
            {
                Message = "/api/ppk2/getportsnames",
                Id = Guid.NewGuid()
            };

            var response = await SendApiRequest(request);
            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    var r = JsonSerializer.Deserialize<List<GetPortsResponse>>(response);
                    return r;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing response: {ex.Message}");
                }
            }

            return new List<GetPortsResponse>();
        }

        /// <summary>
        /// Initialize PPK2
        /// </summary>
        /// <param name="portname"></param>
        /// <returns></returns>
        public async Task<string> PPK2_Init(string portname)
        {
            var request = new DriversWebSocketRequest()
            {
                Message = $"/api/ppk2/init/{portname}",
                Id = Guid.NewGuid()
            };

            var response = await SendApiRequest(request);

            return response;
        }

        /// <summary>
        /// Set PPK2 output voltage
        /// </summary>
        /// <param name="voltage">in millivolts</param>
        /// <returns></returns>
        public async Task<string> PPK2_SetVoltage(int voltage)
        {
            var request = new DriversWebSocketRequest()
            {
                Message = $"/api/ppk2/setvoltage/{voltage}",
                Id = Guid.NewGuid()
            };

            var response = await SendApiRequest(request);

            return response;
        }

        /// <summary>
        /// Turn on PPK2
        /// </summary>
        /// <returns></returns>
        public async Task<string> PPK2_TurnOn()
        {
            var request = new DriversWebSocketRequest()
            {
                Message = $"/api/ppk2/turnon",
                Id = Guid.NewGuid()
            };

            var response = await SendApiRequest(request);

            return response;
        }

        /// <summary>
        /// Turn off PPK2
        /// </summary>
        /// <returns></returns>
        public async Task<string> PPK2_TurnOff()
        {
            var request = new DriversWebSocketRequest()
            {
                Message = $"/api/ppk2/turnoff",
                Id = Guid.NewGuid()
            };
            var response = await SendApiRequest(request);
            return response;
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
            var request = new DriversWebSocketRequest()
            {
                Message = $"/api/ppk2/devicestatus",
                Id = Guid.NewGuid()
            };

            var response = await SendApiRequest(request);

            if (!string.IsNullOrEmpty(response))
            {
                return response == "true";
            }
            return false;
        }

        /// <summary>
        /// Get PPK2 Device Voltage
        /// </summary>
        /// <returns></returns>
        public async Task<int> PPK2_DeviceVoltage()
        {
            var request = new DriversWebSocketRequest()
            {
                Message = $"/api/ppk2/devicevoltage",
                Id = Guid.NewGuid()
            };
            var response = await SendApiRequest(request);
            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    var num = int.Parse(response);
                    return num;
                }
                catch(Exception ex) 
                {
                    Console.WriteLine($"Error parsing response: {ex.Message}");
                }
            }
            return 0;
        }

        /// <summary>
        /// Initialize JLink
        /// </summary>
        /// <param name="withPPK2"></param>
        /// <param name="mcu"></param>
        /// <param name="speed"></param>
        /// <param name="rttaddr"></param>
        /// <returns></returns>
        public async Task<string> JLink_Init(bool withPPK2 = true, string mcu = "nRF52840_xxAA", int speed = 4000, uint rttaddr = 0)
        {
            var request = new DriversWebSocketRequest()
            {
                Message = $"/api/jlink/init/{withPPK2}/{mcu}/{speed}/{rttaddr}",
                Id = Guid.NewGuid()
            };
            var response = await SendApiRequest(request);
            return response;
        }

        /// <summary>
        /// Stop JLink
        /// </summary>
        /// <returns></returns>
        public async Task<string> JLink_Stop()
        {
            var request = new DriversWebSocketRequest()
            {
                Message = $"/api/jlink/stop",
                Id = Guid.NewGuid()
            };
            var response = await SendApiRequest(request);
            return response;
        }

        /// <summary>
        /// Send JLink Command by Channel
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<string> JLink_SendCommandByChannel(int channel, string message)
        {
            message = System.Net.WebUtility.UrlEncode(message);
            var request = new DriversWebSocketRequest()
            {
                Message = $"/api/jlink/sendcommandbychannel/{channel}/{message}",
                Id = Guid.NewGuid()
            };
            var response = await SendApiRequest(request);
            return response;
        }

        /// <summary>
        /// Send JLink Command by Name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<string> JLink_SendCommandByName(string name, string message)
        {
            message = System.Net.WebUtility.UrlEncode(message);
            var request = new DriversWebSocketRequest()
            {
                Message = $"/api/jlink/sendcommandbyname/{name}/{message}",
                Id = Guid.NewGuid()
            };
            var response = await SendApiRequest(request);
            return response;
        }

        /// <summary>
        /// Load all commands from JLink Help
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public async Task<string> JLink_LoadAllCommandsFromHelp(int channel, string parent)
        {
            var uri = $"/api/jlink/loadcommandsfromhelp/{channel}";
            if (!string.IsNullOrEmpty(parent))
            {
                uri += $"/{parent}";
            }
            var request = new DriversWebSocketRequest()
            {
                Message = uri,
                Id = Guid.NewGuid()
            };
            var timeout = 30000;
            if (parent == "")
            {
                timeout = 80000;
            }
            var response = await SendApiRequest(request, timeout);
            return response;
        }

        /// <summary>
        /// Upload Firmware to JLink
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<string> JLink_UploadFirmware(string hash, string filename)
        {
            var request = new DriversWebSocketRequest();
            var timeout = 60000;
            if (filename == "")
            {
                timeout = 120000;
            }

            if (string.IsNullOrEmpty(hash) && !string.IsNullOrEmpty(filename))
            {
                filename = System.Net.WebUtility.UrlEncode(filename);
                var u = $"/api/jlink/uploadfirmwarebyfilename/{filename}";
                request.Message = u;
                request.Id = Guid.NewGuid();
            }
            else if (!string.IsNullOrEmpty(hash) && string.IsNullOrEmpty(filename))
            {

                var u = $"/api/jlink/uploadfirmwarebyhash/{hash}";
                request.Message = u;
                request.Id = Guid.NewGuid();
            }
            var response = await SendApiRequest(request, timeout);
            return response;
        }
    }
}

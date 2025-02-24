using EmbedIO.Sessions;
using hio_dotnet.HWDrivers.MCU;
using Swan.Formatters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.Server
{
    public class RemoteWebSocketClient
    {
        private ClientWebSocket _webSocket;

        public event Action<string> OnMessageReceived;
        public event Action<string> OnCommandReceived;

        public Guid UserId { get; set; } = Guid.NewGuid();
        public Guid SessionId { get; set; } = Guid.Empty;

        public string RemoteWSForwaredServerUrl { get; set; } = "https://thingsboard.hardwario.com/wssessionserver";
        public string RemoteWSForwaredServerJWT { get; set; } = string.Empty;

        public ConcurrentDictionary<Guid, DriversWebSocketRequest> Requests { get; set; } = new ConcurrentDictionary<Guid, DriversWebSocketRequest>();
        public ConcurrentDictionary<Guid, DriversWebSocketResponse> Responses { get; set; } = new ConcurrentDictionary<Guid, DriversWebSocketResponse>();
        public async Task ConnectAsync(string uri)
        {
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
            Console.WriteLine("Connected to WebSocket server");

            if (_webSocket?.State == WebSocketState.Open)
            {
                ;
            }
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
                    if (message != null && message.Contains("WSSessionMessage:"))
                    {
                        message = message.Replace("WSSessionMessage:", "");

                        var wssm = JsonSerializer.Deserialize<WSSessionMessage>(message);
                        if (wssm != null)
                        {
                            if (wssm.Message.Contains("DriversWebSocketModuleApiResponse:"))
                            {
                                var msg = wssm.Message.Replace("DriversWebSocketModuleApiResponse:", "");
                                var response = JsonSerializer.Deserialize<DriversWebSocketResponse>(msg);
                                if (response != null)
                                {
                                    if (Responses.ContainsKey(response.Id))
                                        Responses.TryRemove(response.Id, out var r);
                                    Responses.TryAdd(response.Id, response);

                                }
                            }
                            else if (wssm.Message.Contains("Command:"))
                            {
                                OnCommandReceived?.Invoke(wssm.Message.Replace("Command:", ""));
                            }
                            OnMessageReceived?.Invoke(wssm.Message);
                        }
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

        public async Task<bool> SendWSSessionMessage(string data)
        {
            var sessionMessage = new WSSessionMessage
            {
                SessionId = SessionId,
                UserId = UserId,
                Message = $"{data}"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(sessionMessage);

            try
            {
                await SendMessageAsync($"WSSessionMessage:{json}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending WSSessionMessage: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Send API Request to Drivers Server and wait for the response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="timeout">base delay is 100ms. for example 30s is timeout=300</param>
        /// <returns></returns>
        public async Task<string> SendApiRequest(DriversWebSocketRequest request, int timeout = 300)
        {
            if (SessionId == Guid.Empty)
            {
                throw new Exception("SessionId is not set");
            }

            var json = JsonSerializer.Serialize(request);
            Requests.TryAdd(request.Id, request);

            var sessionMessage = new WSSessionMessage
            {
                SessionId = SessionId,
                UserId = UserId,
                Message = $"DriversWebSocketModuleApiRequest:{json}"
            };

            json = System.Text.Json.JsonSerializer.Serialize(sessionMessage);

            await SendMessageAsync($"WSSessionMessage:{json}");

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
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing response: {ex.Message}");
                }
            }
            return 0;
        }

        public async Task<string> JLink_Init()
        {
            var request = new DriversWebSocketRequest()
            {
                Message = $"/api/jlink/init",
                Id = Guid.NewGuid()
            };
            var response = await SendApiRequest(request);
            return response;
        }

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

        public async Task<string> JLink_SendCommandByChannel(int channel, string message)
        {
            var request = new DriversWebSocketRequest()
            {
                Message = $"/api/jlink/sendcommandbychannel/{channel}/{message}",
                Id = Guid.NewGuid()
            };
            var response = await SendApiRequest(request);
            return response;
        }

        public async Task<string> JLink_SendCommandByName(string name, string message)
        {
            var request = new DriversWebSocketRequest()
            {
                Message = $"/api/jlink/sendcommandbyname/{name}/{message}",
                Id = Guid.NewGuid()
            };
            var response = await SendApiRequest(request);
            return response;
        }

        public async Task<bool> SendCommand(string command)
        {
            try
            {
                await SendWSSessionMessage($"Command:{command}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending command: {ex.Message}");
            }
            return false;
        }

        public async Task<string> LoginToWSForwarderServer(string login, string password)
        {
            using var client = new HttpClient();
            var httpContent = new StringContent(JsonSerializer.Serialize(new { username = login, password = password }), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{RemoteWSForwaredServerUrl}/login", httpContent);
            var jwtToken = await response.Content.ReadAsStringAsync();
            RemoteWSForwaredServerJWT = jwtToken;
            return jwtToken;
        }

        public async Task<string> OpenSessionOnWSForwarderServer(string jwtToken = "")
        {
            if (string.IsNullOrWhiteSpace(jwtToken) && !string.IsNullOrEmpty(RemoteWSForwaredServerJWT))
                jwtToken = RemoteWSForwaredServerJWT;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var response = await client.GetAsync($"{RemoteWSForwaredServerUrl}/addsession");
            var sessionId = await response.Content.ReadAsStringAsync();
            try
            {
                SessionId = Guid.Parse(sessionId.Replace("\"",""));
                return sessionId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing SessionId: {ex.Message}");
            }
            return string.Empty;
        }
    }
}

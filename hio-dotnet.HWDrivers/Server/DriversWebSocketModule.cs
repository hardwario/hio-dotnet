using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Xml.Linq;
using EmbedIO.WebSockets;
using hio_dotnet.HWDrivers;
using hio_dotnet.HWDrivers.MCU;
using Org.BouncyCastle.Asn1;

namespace hio_dotnet.HWDrivers.Server
{
    public class WebSocketModuleTransferObject
    {
        public string Message { get; set; } = string.Empty;
        public MultiRTTClientBase ClientBase { get; set; } = new MultiRTTClientBase();
    }

    public class DriversWebSocketRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Message { get; set; } = string.Empty;

        public bool IsProcessed { get; set; } = false;

        public bool IsResponseSent { get; set; } = false;
    }

    public class DriversWebSocketResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Response { get; set; } = string.Empty;
    }

    public class DriversWebSocketModule : WebSocketModule
    {
        private int _connectedClients = 0;
        private bool _subscribed = false;
        public DriversWebSocketModule(string urlPath)
            : base(urlPath, true)
        {

            // subscribe to info about jslink was connected to subscribe to RTT messages
            DriversServerMainDataContext.OnJLinkConnected += DriversServerMainDataContext_OnJLinkConnected;
        }

        private void DriversServerMainDataContext_OnJLinkConnected(object? sender, EventArgs e)
        {
            if (!_subscribed)
            {
                if (DriversServerMainDataContext.MCUMultiRTTConsole != null)
                {
                    DriversServerMainDataContext.MCUMultiRTTConsole.NewRTTMessageLineReceived += async (sender, e) =>
                    {
                        if (e != null)
                        {
                            var m = new WebSocketModuleTransferObject()
                            {
                                Message = e.Item1,
                                ClientBase = e.Item2
                            };
                            var json = System.Text.Json.JsonSerializer.Serialize(m);
                            // send json to all
                            //Console.WriteLine($"Message Broadcast: {json}");
                            await BroadcastAsync(json);
                        }
                    };
                    _subscribed = true;
                }
            }
        }

        protected override async Task OnClientConnectedAsync(IWebSocketContext context)
        {
            _connectedClients++;
            Console.WriteLine($"Connected {_connectedClients} clients.");
        }

        protected override async Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
        {
            var message = System.Text.Encoding.UTF8.GetString(buffer);
            Console.WriteLine($"Received on drivers server websocket: {message}");

            if (message.Contains("DriversWebSocketModuleApiRequest:"))
            {
                var parts = message.Split("DriversWebSocketModuleApiRequest:");
                if (parts.Length == 2)
                {
                    try
                    {
                        var parsed = System.Text.Json.JsonSerializer.Deserialize<DriversWebSocketRequest>(parts[1]);
                        if (parsed != null)
                        {
                            using (var httpClient = new HttpClient())
                            {
                                httpClient.BaseAddress = new System.Uri(DriversServerMainDataContext.ServerBaseUrl);
                                message = System.Web.HttpUtility.UrlEncode(message);

                                var url = parsed.Message;
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

                                    var responseMessage = new DriversWebSocketResponse()
                                    {
                                        Id = parsed.Id,
                                        Response = responseBody
                                    };

                                    var json = System.Text.Json.JsonSerializer.Serialize(responseMessage);

                                    await SendAsync(context, $"DriversWebSocketModuleApiResponse:{json}");
                                }
                                catch (HttpRequestException ex)
                                {
                                    throw new Exception("An error occurred while sending command by name to jlink.", ex);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to parse message as DriversWebSocketRequest: {ex.Message}");
                    }
                }
                if (DriversServerMainDataContext.MCUMultiRTTConsole != null)
                {
                    try
                    {
                        var msg = System.Text.Json.JsonSerializer.Deserialize<MultiRTTClientWSMessage>(message);
                        if (msg == null)
                        {
                            Console.WriteLine("Failed to parse message as MultiRTTClientWSMessage");
                            await SendAsync(context, "Message is not valid MultiRTTClientWSMessage");
                            return;
                        }
                        await DriversServerMainDataContext.MCUMultiRTTConsole.SendCommand(msg.Name, msg.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to parse message as MultiRTTClientWSMessage: {ex.Message}");
                    }
                }
            }
        }

        protected override async Task OnClientDisconnectedAsync(IWebSocketContext context)
        {
            _connectedClients--;
            Console.WriteLine($"Connected {_connectedClients} clients.");
        }
    }
}


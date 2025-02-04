using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
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

        protected override async Task OnClientDisconnectedAsync(IWebSocketContext context)
        {
            _connectedClients--;
            Console.WriteLine($"Connected {_connectedClients} clients.");
        }
    }
}


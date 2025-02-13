using hio_dotnet.HWDrivers.Server;
using System.Net.WebSockets;
using System.Text;

namespace hio_dotnet.Demos.WSSessionsServer
{
    public static class WebSocketProcessor
    {
        public static async Task HandleWebSocketAsync(WebSocket webSocket, HttpContext context)
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;
                try
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving message: {ex.Message}");
                    break;
                }
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
                    break;
                }

                var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received: {receivedMessage}");

                if (!string.IsNullOrWhiteSpace(receivedMessage) && receivedMessage.Contains("WSSessionMessage:"))
                {
                    var split = receivedMessage.Split("WSSessionMessage:");
                    try
                    {
                        var message = System.Text.Json.JsonSerializer.Deserialize<WSSessionMessage>(split[1]);
                        if (message != null)
                        {
                            if (MainDataContext.Sessions.TryGetValue(message.SessionId.ToString(), out var session))
                            {
                                if (session.User1Id == Guid.Empty)
                                {
                                    session.User1Id = message.UserId;
                                    session.User1Socket = webSocket;
                                }
                                else if (session.User2Id == Guid.Empty)
                                {
                                    session.User2Id = message.UserId;
                                    session.User2Socket = webSocket;
                                }

                                MainDataContext.Sessions.TryRemove(session.Id.ToString(), out _);
                                MainDataContext.Sessions.TryAdd(session.Id.ToString(), session);

                                if (session.User1Socket != null && session.User2Socket != null)
                                {
                                    var forwardMessage = $"WSSessionMessage:{split[1]}";
                                    var responseBytes = Encoding.UTF8.GetBytes(forwardMessage);

                                    var targetSocket = message.UserId == session.User1Id ? session.User2Socket : session.User1Socket;
                                    if (targetSocket != null && targetSocket.State == WebSocketState.Open)
                                    {
                                        await targetSocket.SendAsync(
                                            new ArraySegment<byte>(responseBytes, 0, responseBytes.Length),
                                            WebSocketMessageType.Text,
                                            true,
                                            CancellationToken.None);
                                    }
                                }
                            }
                            else
                            {
                                var responseBytes = Encoding.UTF8.GetBytes("Error:Session not found");
                                await webSocket.SendAsync(
                                    new ArraySegment<byte>(responseBytes, 0, responseBytes.Length),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing WSSessionMessage: {ex.Message}");
                    }
                }
            }
        }
    }
}

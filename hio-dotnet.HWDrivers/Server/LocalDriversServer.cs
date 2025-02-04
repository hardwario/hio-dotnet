using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Files;
using EmbedIO.WebApi;
using Swan.Logging;
using EmbedIO.WebSockets;
using System.Collections.Concurrent;
using EmbedIO.Cors;

namespace hio_dotnet.HWDrivers.Server
{
    public class LocalDriversServer : IDisposable
    {
        private readonly WebServer _server;

        private ConcurrentDictionary<Guid, Task> ServerTasks { get; set; } = new ConcurrentDictionary<Guid, Task>();

        private Guid mainServiceId = Guid.Empty;
        private Guid mainJLinkId = Guid.Empty;
        private Guid checkJLinkTaskId = Guid.Empty;

        public LocalDriversServer(string? resourcePath, int port = 8042)
        {
            _server = new WebServer(o => o
                            .WithUrlPrefix($"http://localhost:{port}")
                            .WithMode(HttpListenerMode.EmbedIO))
                            .WithLocalSessionManager()
                            .WithCors(origins: "*", headers: "*", methods: "*")
                            .WithWebApi("/api", m => m
                    .WithController<DriversApiControler>())
                            .WithModule(new DriversWebSocketModule("/ws"));

            if (!string.IsNullOrEmpty(resourcePath))
            {
                Console.WriteLine($"Serving files from: {resourcePath}");
                _server.WithStaticFolder("/", resourcePath, true);
            }
        }


        /// <summary>
        /// Start server
        /// </summary>
        public Task Start()
        {
            mainServiceId = Guid.NewGuid();
            ServerTasks.TryAdd(mainServiceId, _server.RunAsync());

            checkJLinkTaskId = Guid.NewGuid();
            ServerTasks.TryAdd(mainServiceId, CheckJLinkTask()); 

            Console.WriteLine("Server running at http://localhost:{port}");

            return RunServer();
        }

        public Task CheckJLinkTask()
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                    if (DriversServerMainDataContext.MCUMultiRTTConsole != null)
                    {
                        if (!DriversServerMainDataContext.JLinkTaskQueue.TryDequeue(out Task jlinktask))
                        {
                            if (jlinktask != null)
                            {
                                mainJLinkId = Guid.NewGuid();
                                ServerTasks.TryAdd(mainJLinkId, jlinktask);
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Await all simulators
        /// </summary>
        /// <returns></returns>
        public Task RunServer()
        {
            return Task.Run(async () =>
            {
                while (ServerTasks.Count > 0)
                {
                    // Await any non-completed tasks in the dictionary of Simulator Tasks
                    var completedTask = await Task.WhenAny(ServerTasks.Values);

                    // Remove completed task from the dictionary
                    var completedId = ServerTasks.FirstOrDefault(x => x.Value == completedTask).Key;
                    if (completedId != Guid.Empty)
                    {
                        //Console.WriteLine($"[INFO] Simulator with ID {completedId} has completed.");
                        ServerTasks.TryRemove(completedId, out _);
                    }
                }
            });
        }

        /// <summary>
        /// Stop server and all running tasks
        /// </summary>
        public void Stop()
        {
            if (DriversServerMainDataContext.cts != null)
            {
                DriversServerMainDataContext.cts.Cancel();
            }

            if (DriversServerMainDataContext.PPK2_Driver != null)
            {
                DriversServerMainDataContext.PPK2_Driver.Dispose();
            }

            if (DriversServerMainDataContext.MCUMultiRTTConsole != null)
            {
                DriversServerMainDataContext.MCUMultiRTTConsole.Dispose();
            }

            foreach (var task in ServerTasks.Values)
            {
                task.Dispose();
            }
        }
        public void Dispose()
        {
            Stop();
        }

    }
}

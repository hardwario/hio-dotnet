using hio_dotnet.APIs.HioCloud.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloud
{
    public class CloudMessagesGrabbersHandler
    {
        private ConcurrentDictionary<Guid, ICloudMessagesGrabber> Grabbers { get; set; } = new ConcurrentDictionary<Guid, ICloudMessagesGrabber>();
        private ConcurrentDictionary<Guid, Task> GrabbersTasks { get; set; } = new ConcurrentDictionary<Guid, Task>();

        public int NumberOfRunningSimulators => GrabbersTasks.Count;
        /// <summary>
        /// Event which fires anytime when a new message is grabbed by any grabber
        /// </summary>
        public event EventHandler<CloudMessagesGrabberEventArgs> OnNewDataReceived;

        private void OnNewDataReceivedHandler(object sender, CloudMessagesGrabberEventArgs args)
        {
            OnNewDataReceived?.Invoke(sender, args);
        }

        public Guid AddNewGrabber(Guid spaceId,
                                  Guid deviceId,
                                  string cloudUrl,
                                  long interval,
                                  string name,
                                  string? username = null,
                                  string? password = null,
                                  string? jwtToken = null,
                                  string? apitoken = null,
                                  string description = "",
                                  string externalSoftwareId = "",
                                  string externalSoftwareAccessToken = "",
                                  bool start = true)
        {
            SimpleCloudMessageGrabber? grab = null;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                grab = new SimpleCloudMessageGrabber(spaceId, deviceId, cloudUrl, username, password);
            }
            else if (!string.IsNullOrEmpty(jwtToken))
            {
                grab = new SimpleCloudMessageGrabber(spaceId, deviceId, cloudUrl, jwtToken: jwtToken);
            }
            else if (!string.IsNullOrEmpty(apitoken))
            {
                grab = new SimpleCloudMessageGrabber(spaceId, deviceId, cloudUrl, apitoken: apitoken, useapitoken: true);
            }
            else if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password) && string.IsNullOrEmpty(jwtToken) && string.IsNullOrEmpty(apitoken))
            {
                throw new ArgumentNullException("You must fill username and password or jwtToken or API token to start HARDWARIO Cloud Grabber.");
            }
            else
            {
                return Guid.Empty;
            }

            if (grab == null)
                return Guid.Empty;
            grab.SetInterval(interval);
            grab.Name = name;

            grab.OnNewDataReceived += OnNewDataReceivedHandler;

            if (!string.IsNullOrEmpty(description))
                grab.Description = description;
            if (!string.IsNullOrEmpty(externalSoftwareId))
                grab.ExternalSoftwareId = externalSoftwareId;
            if (!string.IsNullOrEmpty(externalSoftwareAccessToken))
                grab.ExternalSoftwareAccessToken = externalSoftwareAccessToken;

            if (Grabbers.TryAdd(grab.Id, grab))
            {
                if (start)
                {
                    var task = grab.Start();
                    GrabbersTasks.TryAdd(grab.Id, task);
                }
                return grab.Id;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Login to the cloud with the specific grabber in the case you are using username and password.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> Login(Guid id, string username, string password)
        {
            if (Grabbers.TryGetValue(id, out var grab))
            {
                return await grab.Login(username, password);

            }
            return false;
        }

        /// <summary>
        /// Start graber if it is not running already.
        /// This function will add Task to the internal GrabbersTasks dictionary.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool StartGrabber(Guid id)
        {
            if (Grabbers.TryGetValue(id, out var grab))
            {
                if (grab.IsRunning)
                    return true;
                if (GrabbersTasks.ContainsKey(id))
                    return true;

                var task = grab.Start();
                return GrabbersTasks.TryAdd(id, task);
            }
            return false;
        }

        /// <summary>
        /// Stop grabber and remove Task from the internal GrabbersTasks dictionary.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public async Task<bool> StopGrabber(Guid id)
        {
            if (Grabbers.TryGetValue(id, out var grab))
            {
                if (!grab.IsRunning)
                {
                    //Console.WriteLine($"[INFO] Grabber {id} is not running.");
                    return true;
                }

                //Console.WriteLine($"[INFO] Stopping grabber {id}...");
                await grab.Stop();

                if (GrabbersTasks.TryGetValue(id, out var task))
                {
                    try
                    {
                        //Console.WriteLine($"[INFO] Waiting for {id} task completed.");
                        await task;
                        Console.WriteLine($"[INFO] Grabber {id} Stopped.");
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine($"[INFO] Grabber {id} task was canceled.");
                    }
                    return GrabbersTasks.TryRemove(id, out _);
                }
            }
            return false;
        }

        /// <summary>
        /// Remove grabber from the dictionary.
        /// Function will stop grabber if it is running and it will remove the event handler.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> RemoveGrabber(Guid id)
        {
            if (Grabbers.TryGetValue(id, out var grab))
            {
                await StopGrabber(id);
                grab.OnNewDataReceived -= OnNewDataReceivedHandler;

                return Grabbers.TryRemove(id, out _);
            }

            return false;
        }

        /// <summary>
        /// Get specific grabber by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ICloudMessagesGrabber? GetGrabber(Guid id)
        {
            if (Grabbers.TryGetValue(id, out var grab))
                return grab;
            return null;
        }

        /// <summary>
        /// Get all grabbers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICloudMessagesGrabber> GetAllGrabbers()
        {
            return Grabbers.Values;
        }

        /// <summary>
        /// Stop All grabbers
        /// </summary>
        public Task StopAllGrabbers()
        {
            return Task.Run(async () =>
            {
                const int batchSize = 1000;

                while (GrabbersTasks.Count > 0)
                {
                    var grabbersToStop = GrabbersTasks.Take(batchSize).Select(x => x.Key).ToList();

                    var stopTasks = grabbersToStop.Select(id => StopGrabber(id));
                    await Task.WhenAll(stopTasks);
                }
            });
        }

        /// <summary>
        /// Await all grabbers
        /// </summary>
        /// <returns></returns>
        public Task MonitorGrabbers()
        {
            return Task.Run(async () =>
            {
                while (GrabbersTasks.Count > 0)
                {
                    // Await any non-completed tasks in the dictionary of Grabber Tasks
                    var completedTask = await Task.WhenAny(GrabbersTasks.Values);

                    // Remove completed task from the dictionary
                    var completedId = GrabbersTasks.FirstOrDefault(x => x.Value == completedTask).Key;
                    if (completedId != Guid.Empty)
                    {
                        //Console.WriteLine($"[INFO] Grabber with ID {completedId} has completed.");
                        GrabbersTasks.TryRemove(completedId, out _);
                    }
                }
            });
        }
    }
}

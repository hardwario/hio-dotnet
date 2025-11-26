using hio_dotnet.APIs.HioCloud.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloud
{
    public class SimpleCloudMessageGrabber : CloudMessagesGrabberBase
    {
        public SimpleCloudMessageGrabber(Guid spaceId, Guid deviceId, string baseUrl, string username, string password, int port = 0)
        {
            CloudDriver = new HioCloudDriver(baseUrl, username, password, port);
            SpaceId = spaceId;
            DeviceId = deviceId;
        }

        public SimpleCloudMessageGrabber(Guid spaceId, Guid deviceId, string baseUrl, string jwtToken, int port = 0)
        {
            CloudDriver = new HioCloudDriver(baseUrl, jwtToken, port);
            SpaceId = spaceId;
            DeviceId = deviceId;
        }

        public SimpleCloudMessageGrabber(Guid spaceId, Guid deviceId, string baseUrl, string apitoken, bool useapitoken = true, int port = 0)
        {
            CloudDriver = new HioCloudDriver(baseUrl, apitoken, useapitoken, port);
            SpaceId = spaceId;
            DeviceId = deviceId;
        }

        /// <summary>
        /// Id of the grabber
        /// </summary>
        public override Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Id of the Space where should device is place in the cloud
        /// </summary>
        public override Guid SpaceId { get; set; }
        /// <summary>
        /// Id of the Device in the cloud
        /// </summary>
        public override Guid DeviceId { get; set; }

        private bool _isRunning = false;
        /// <summary>
        /// Indicates if the grabber is running
        /// </summary>
        public override bool IsRunning => _isRunning;
        /// <summary>
        /// Name of the grabber
        /// </summary>
        public override string Name { get; set; } = string.Empty;
        /// <summary>
        /// Description of the grabber
        /// </summary>
        public override string Description { get; set; } = string.Empty;
        /// <summary>
        /// The Id of simulated device in some external software where the message should go to
        /// </summary>
        public override string? ExternalSoftwareId { get; set; }
        /// <summary>
        /// The Access Token of simulated device in some external software where the message should go to
        /// </summary>
        public override string? ExternalSoftwareAccessToken { get; set; }
        /// <summary>
        /// Maximum number of messages in the storage
        /// </summary>
        public override int MaximumStoredMessages { get; set; } = 10;
        /// <summary>
        /// Actual Number of messages stored in the history of the grabber
        /// </summary>
        public override int MessagesCount => _messages.Count;
        /// <summary>
        /// Interval of the grabber cycle in milliseconds
        /// This intervals means that grabber will try to ask cloud for the new messages. It does not mean that the cloud will send the messages in this interval.
        /// </summary>
        public override long Interval { get; set; } = 5000;
        /// <summary>
        /// Last message received to the grabber from the cloud
        /// </summary>
        public override HioCloudMessage? LastMessage { get; set; }
        /// <summary>
        /// This event will occur when the new message is received from the cloud
        /// </summary>
        public override event EventHandler<CloudMessagesGrabberEventArgs>? OnNewDataReceived;
        /// <summary>
        /// Async version of event which fires anytime when a new message is grabbed from cloud
        /// </summary>
        public override event Func<object?, HioCloudMessage, Task>? OnNewDataAsync;
        /// <summary>
        /// This event will occur when the grabber is started
        /// </summary>
        public override event EventHandler<Guid>? OnGrabberStarted;
        /// <summary>
        /// This event will occur when the grabber is stopped
        /// </summary>
        public override event EventHandler<Guid>? OnGrabberEnded;
        /// <summary>
        /// Happens when log event will happen
        /// </summary>
        public override event EventHandler<(Guid, string, Exception?)>? OnLogHappened;
        /// <summary>
        /// Cloud driver that will be used to communicate with the cloud
        /// </summary>
        public override HioCloudDriver? CloudDriver { get; set; }

        private CancellationTokenSource? _cancellationTokenSource;

        private ConcurrentDictionary<Guid, HioCloudMessage> _messages { get; set; } = new ConcurrentDictionary<Guid, HioCloudMessage>();

        #region FluentAPI

        public SimpleCloudMessageGrabber WithName(string name)
        {
            Name = name;
            return this;
        }
        public SimpleCloudMessageGrabber WithDescription(string description)
        {
            Description = description;
            return this;
        }
        public SimpleCloudMessageGrabber WithExternalSoftwareId(string externalSoftwareId)
        {
            ExternalSoftwareId = externalSoftwareId;
            return this;
        }
        public SimpleCloudMessageGrabber WithExternalSoftwareAccessToken(string externalSoftwareAccessToken)
        {
            ExternalSoftwareAccessToken = externalSoftwareAccessToken;
            return this;
        }
        public SimpleCloudMessageGrabber WithInterval(long interval)
        {
            SetInterval(interval);
            return this;
        }
        #endregion

        public override async Task<bool> Login(string username, string password)
        {
            var jwt = await CloudDriver?.Login(username, password);
            if (jwt != null)
            {
                return true;
            }
            return false;
        }
        public override HioCloudMessage? GetLastMessage()
        {
            return LastMessage;
        }
        /// <summary>
        /// Set interval when grabber should try to check the cloud for the new message
        /// </summary>
        /// <param name="interval"></param>
        public override void SetInterval(long interval)
        {
            if (interval > 0)
                Interval = interval;
            else
                throw new Exception("Interval must be greater than 0");
        }
        /// <summary>
        /// Start the data grabbing process
        /// </summary>
        /// <returns></returns>
        public override Task Start(CancellationTokenSource? cts = null)
        {
            if (cts != null)
            {
                _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
            }
            else
            {
                _cancellationTokenSource = new CancellationTokenSource();
            }

            return RunGrabber(_cancellationTokenSource.Token);
        }
        /// <summary>
        /// Get Last message received from the cloud
        /// </summary>
        /// <returns></returns>
        public override async Task Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        private Task RunGrabber(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                _isRunning = true;
                OnGrabberStarted?.Invoke(this, Id);

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        HioCloudMessage? receivedMessage = null;
                        // get the last message from the cloud
                        try
                        {
                            OnLogHappened?.Invoke(this, (Id, $"Grabber: {Name} is checking the new messages on cloud...", null));
                            receivedMessage = CloudDriver?.GetAllDeviceMessages(SpaceId, DeviceId)?.Result?.FirstOrDefault();
                        }
                        catch (Exception ex) 
                        {
                            OnLogHappened?.Invoke(this, (Id, $"Grabber: {Name} cannot get message from the Hio Cloud, Exception Message: {ex.Message}", ex));
                        }

                        if (receivedMessage != null)
                        {
                            // check if the message is still the same. If yes continue waiting for the new message
                            if (receivedMessage.Id == Guid.Empty ||
                                receivedMessage.Id == LastMessage?.Id ||
                                _messages.ContainsKey(receivedMessage.Id))
                            {
                                OnLogHappened?.Invoke(this, (Id, $"Grabber: {Name} >>> No new messages on cloud for the device: {DeviceId}.", null));
                                await Task.Delay((int)Interval, cancellationToken);

                                continue;
                            }

                            _messages.TryAdd(receivedMessage.Id, receivedMessage);

                            if (_messages.TryGetValue(receivedMessage.Id, out var msgstored))
                                LastMessage = msgstored;

                            // if the _messages count is greater than MaximumStoredMessages, remove 10% of oldest messages
                            // this will decrease load to CPU because it will not happen in each cycle
                            var toTake = (int)((double)MaximumStoredMessages * 0.1);
                            if (MaximumStoredMessages < 5 || toTake < 1)
                                toTake = 1;

                            if (_messages.Count > MaximumStoredMessages)
                            {
                                var oldestMessages = _messages.OrderBy(x => x.Value.CreatedAt)
                                                              .Take(toTake)
                                                              .Select(x => x.Key)
                                                              .ToList();

                                foreach (var key in oldestMessages)
                                {
                                    _messages.TryRemove(key, out var _);
                                }
                            }

                            OnNewDataReceived?.Invoke(this, new CloudMessagesGrabberEventArgs()
                            {
                                GrabberId = Id,
                                Message = receivedMessage
                            });

                            if (OnNewDataAsync != null)
                            {
                                try
                                {
                                    await OnNewDataAsync(this, receivedMessage);
                                }
                                catch (Exception ex)
                                {
                                    OnLogHappened?.Invoke(this, (Id, $"Grabber: {Name} cannot send message to the external software, Exception Message: {ex.Message}", ex));
                                }
                            }
                        }

                        await Task.Delay((int)Interval, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        OnLogHappened?.Invoke(this, (Id, $"Exception in Grabber: {Name}, ID: {Id}, Exception Message: {ex.Message}", ex));
                    }

                }

                _isRunning = false;
                OnGrabberEnded?.Invoke(this, Id);
                _cancellationTokenSource?.Dispose();

            }, cancellationToken);
        }
    }
}

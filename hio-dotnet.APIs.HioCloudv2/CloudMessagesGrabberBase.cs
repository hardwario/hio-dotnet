using hio_dotnet.APIs.HioCloud.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloud
{
    public abstract class CloudMessagesGrabberBase : ICloudMessagesGrabber
    {
        /// <summary>
        /// Id of the grabber
        /// </summary>
        public abstract Guid Id { get; set; }
        /// <summary>
        /// Id of the Space where should device is place in the cloud
        /// </summary>
        public abstract Guid SpaceId { get; set; }
        /// <summary>
        /// Id of the Device in the cloud
        /// </summary>
        public abstract Guid DeviceId { get; set; }
        /// <summary>
        /// Indicates if the grabber is running
        /// </summary>
        public abstract bool IsRunning { get; }
        /// <summary>
        /// Name of the grabber
        /// </summary>
        public abstract string Name { get; set; }
        /// <summary>
        /// Description of the grabber
        /// </summary>
        public abstract string Description { get; set; }
        /// <summary>
        /// The Id of simulated device in some external software where the message should go to
        /// </summary>
        public abstract string? ExternalSoftwareId { get; set; }
        /// <summary>
        /// The Access Token of simulated device in some external software where the message should go to
        /// </summary>
        public abstract string? ExternalSoftwareAccessToken { get; set; }
        /// <summary>
        /// Maximum number of messages in the storage
        /// </summary>
        public abstract int MaximumStoredMessages { get; set; }
        /// <summary>
        /// Actual Number of messages stored in the history of the grabber
        /// </summary>
        public abstract int MessagesCount { get; }
        /// <summary>
        /// Interval of the grabber cycle
        /// This intervals means that grabber will try to ask cloud for the new messages. It does not mean that the cloud will send the messages in this interval.
        /// </summary>
        public abstract long Interval { get; set; }
        /// <summary>
        /// Last message received to the grabber from the cloud
        /// </summary>
        public abstract HioCloudMessage? LastMessage { get; set; }
        /// <summary>
        /// This event will occur when the new message is received from the cloud
        /// </summary>
        public abstract event EventHandler<CloudMessagesGrabberEventArgs>? OnNewDataReceived;
        /// <summary>
        /// This event will occur when the grabber is started
        /// </summary>
        public abstract event EventHandler<Guid>? OnGrabberStarted;
        /// <summary>
        /// This event will occur when the grabber is stopped
        /// </summary>
        public abstract event EventHandler<Guid>? OnGrabberEnded;
        /// <summary>
        /// Cloud driver that will be used to communicate with the cloud
        /// </summary>
        public abstract HioCloudDriver? CloudDriver { get; set; }
        /// <summary>
        /// Start the data grabbing process
        /// </summary>
        /// <returns></returns>
        public abstract Task Start();
        /// <summary>
        /// Stop the data grabbing process
        /// </summary>
        /// <returns></returns>
        public abstract Task Stop();
        /// <summary>
        /// Set interval when grabber should try to check the cloud for the new message
        /// </summary>
        /// <param name="interval"></param>
        public abstract void SetInterval(long interval);
        /// <summary>
        /// Get Last message received from the cloud
        /// </summary>
        /// <returns></returns>
        public abstract HioCloudMessage? GetLastMessage();
    }
}


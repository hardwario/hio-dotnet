using hio_dotnet.APIs.HioCloud.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloud
{
    public interface ICloudMessagesGrabber
    {
        /// <summary>
        /// Id of the grabber
        /// </summary>
        Guid Id { get; set; }
        /// <summary>
        /// Id of the Space where should device is place in the cloud
        /// </summary>
        Guid SpaceId { get; set; }
        /// <summary>
        /// Id of the Device in the cloud
        /// </summary>
        Guid DeviceId { get; set; }
        /// <summary>
        /// Indicates if the grabber is running
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// Name of the grabber
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Description of the grabber
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// The Id of simulated device in some external software where the message should go to
        /// </summary>
        string? ExternalSoftwareId { get; set; }
        /// <summary>
        /// The Access Token of simulated device in some external software where the message should go to
        /// </summary>
        string? ExternalSoftwareAccessToken { get; set; }
        /// <summary>
        /// Maximum number of messages in the storage
        /// </summary>
        int MaximumStoredMessages { get; set; }
        /// <summary>
        /// Actual Number of messages stored in the history of the grabber
        /// </summary>
        int MessagesCount { get; }
        /// <summary>
        /// Interval of the grabber cycle
        /// This intervals means that grabber will try to ask cloud for the new messages. It does not mean that the cloud will send the messages in this interval.
        /// </summary>
        long Interval { get; set; }
        /// <summary>
        /// Last message received to the grabber from the cloud
        /// </summary>
        HioCloudMessage? LastMessage { get; set; }
        /// <summary>
        /// This event will occur when the new message is received from the cloud
        /// </summary>
        event EventHandler<CloudMessagesGrabberEventArgs>? OnNewDataReceived;
        /// <summary>
        /// Async version of event which fires anytime when a new message is grabbed by any grabber
        /// </summary>
        event Func<object?, HioCloudMessage, Task>? OnNewDataAsync;
        /// <summary>
        /// This event will occur when the grabber is started
        /// </summary>
        event EventHandler<Guid>? OnGrabberStarted;
        /// <summary>
        /// This event will occur when the grabber is stopped
        /// </summary>
        event EventHandler<Guid>? OnGrabberEnded;
        /// <summary>
        /// Cloud driver that will be used to communicate with the cloud
        /// </summary>
        HioCloudDriver? CloudDriver { get; set; }
        /// <summary>
        /// Login in case of using username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> Login(string username, string password);
        /// <summary>
        /// Start the data grabbing process
        /// </summary>
        /// <returns></returns>
        Task Start();
        /// <summary>
        /// Stop the data grabbing process
        /// </summary>
        /// <returns></returns>
        Task Stop();
        /// <summary>
        /// Set interval when grabber should try to check the cloud for the new message
        /// </summary>
        /// <param name="interval"></param>
        void SetInterval(long interval);
        /// <summary>
        /// Get Last message received from the cloud
        /// </summary>
        /// <returns></returns>
        HioCloudMessage? GetLastMessage();
    }
}

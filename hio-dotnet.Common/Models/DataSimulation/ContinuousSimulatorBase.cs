﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.DataSimulation
{
    public abstract class ContinuousSimulatorBase<T> : IContinuousSimulator<T> where T : class
    {
        /// <summary>
        /// Unique identifier of the simulator
        /// </summary>
        public abstract Guid Id { get; set; }
        /// <summary>
        /// Identify if the simulator is running
        /// </summary>
        public abstract bool IsRunning { get; }
        /// <summary>
        /// Name of the simulator
        /// </summary>
        public abstract string Name { get; set; }
        /// <summary>
        /// Description of the simulator
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
        /// Number of messages generated by the simulator so far
        /// </summary>
        public abstract int MessagesCount { get; }
        /// <summary>
        /// Interval between messages in milliseconds
        /// </summary>
        public abstract long Interval { get; set; }
        /// <summary>
        /// Latest generated message
        /// </summary>
        public SimulatedMessage<T>? LastMessage { get; set; }

        #region STaticsVariablesOfTheDevice
        /// <summary>
        /// Static Simulator Value of Firmware Version
        /// </summary>
        public abstract string? FwVersion { get; set; }
        /// <summary>
        /// Static Simulator Value of Hardware Version
        /// </summary>
        public abstract string? HwRevision { get; set; }
        /// <summary>
        /// Static Simulator Value of Serial Number
        /// </summary>
        public abstract string? SerialNumber { get; set; }
        /// <summary>
        /// Static Simulator Value of IMEI
        /// </summary>
        public abstract long? IMEI { get; set; }
        /// <summary>
        /// Static Simulator Value of IMSI
        /// </summary>
        public abstract long? IMSI { get; set; }
        #endregion

        /// <summary>
        /// Event raised when a new message is generated
        /// </summary>

        public abstract event EventHandler<ContinuousSimulatorEventArgs> OnDataGenerated;

        public abstract event EventHandler<Guid> OnSimulatorStarted;
        public abstract event EventHandler<Guid> OnSimulatorEnded;

        /// <summary>
        /// Function to get the last message generated by the simulator
        /// This can be helpful to save the latest stage of the simulator message for later reload from file or database
        /// </summary>
        public abstract SimulatedMessage<T>? GetLastMessage();
        /// <summary>
        /// Load the initial message of the simulator
        /// This can be helpful to renew the simulator message from file or database
        /// </summary>
        public abstract void LoadInitMessage(string message);

        /// <summary>
        /// Start Simulator main loop
        /// </summary>
        public abstract Task Start();

        /// <summary>
        /// Stop Simulator main loop
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Set the interval between messages
        /// </summary>
        /// <param name="interval"></param>
        public abstract void SetInterval(long interval);
    }
}

﻿using hio_dotnet.Common.Helpers;
using hio_dotnet.Common.Models.CatalogApps;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.DataSimulation
{
    public class StandardContinuousSimulator<T> : ContinuousSimulatorBase<T> where T : class
    {
        /// <summary>
        /// Unique identifier of the simulator
        /// </summary>
        public override Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Identify if the simulator is running
        /// </summary>
        private bool _isRunning = false;
        public override bool IsRunning { get => _isRunning; }
        /// <summary>
        /// Name of the simulator
        /// </summary>
        public override string Name { get; set; } = string.Empty;
        /// <summary>
        /// Description of the simulator
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
        /// Number of messages generated by the simulator so far
        /// </summary>
        public override int MessagesCount => _messages.Count;
        /// <summary>
        /// Interval between messages in milliseconds
        /// </summary>
        public override long Interval { get; set; } = 5000;
        /// <summary>
        /// Latest generated message
        /// </summary>
        public SimulatedMessage<T>? LastMessage { get; set; }

        #region STaticsVariablesOfTheDevice
        /// <summary>
        /// Static Simulator Value of Firmware Version
        /// </summary>
        public override string? FwVersion { get; set; }
        /// <summary>
        /// Static Simulator Value of Hardware Version
        /// </summary>
        public override string? HwRevision { get; set; }
        /// <summary>
        /// Static Simulator Value of Serial Number
        /// </summary>
        public override string? SerialNumber { get; set; }
        /// <summary>
        /// Static Simulator Value of IMEI
        /// </summary>
        public override long? IMEI { get; set; }
        /// <summary>
        /// Static Simulator Value of IMSI
        /// </summary>
        public override long? IMSI { get; set; }
        #endregion
        /// <summary>
        /// Event raised when a new message is generated
        /// </summary>

        public override event EventHandler<ContinuousSimulatorEventArgs> OnDataGenerated;
        public override event EventHandler<Guid> OnSimulatorStarted;
        public override event EventHandler<Guid> OnSimulatorEnded;

        private CancellationTokenSource _cancellationTokenSource;

        private ConcurrentDictionary<Guid, SimulatedMessage<T>> _messages { get; set; } = new ConcurrentDictionary<Guid, SimulatedMessage<T>>();

        #region FluentAPI
        public StandardContinuousSimulator<T> WithName(string name)
        {
            Name = name;
            return this;
        }

        public StandardContinuousSimulator<T> WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public StandardContinuousSimulator<T> WithInterval(long interval)
        {
            Interval = interval;
            return this;
        }

        public StandardContinuousSimulator<T> WithExternalSoftwareId(string externalSoftwareId)
        {
            ExternalSoftwareId = externalSoftwareId;
            return this;
        }

        public StandardContinuousSimulator<T> WithExternalSoftwareAccessToken(string externalSoftwareAccessToken)
        {
            ExternalSoftwareAccessToken = externalSoftwareAccessToken;
            return this;
        }

        public StandardContinuousSimulator<T> WithFwVersion(string fwVersion)
        {
            FwVersion = fwVersion;
            return this;
        }

        public StandardContinuousSimulator<T> WithHwVersion(string hwVersion)
        {
            HwRevision = hwVersion;
            return this;
        }

        public StandardContinuousSimulator<T> WithSerialNumber(string serialNumber)
        {
            SerialNumber = serialNumber;
            return this;
        }

        public StandardContinuousSimulator<T> WithIMEI(long imei)
        {
            IMEI = imei;
            return this;
        }

        public StandardContinuousSimulator<T> WithIMSI(long imsi)
        {
            IMSI = imsi;
            return this;
        }

        #endregion

        /// <summary>
        /// Set the interval between messages
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
        /// Function to get the last message generated by the simulator
        /// This can be helpful to save the latest stage of the simulator message for later reload from file or database
        /// </summary>
        public override SimulatedMessage<T>? GetLastMessage()
        {
            return LastMessage;
        }

        /// <summary>
        /// Load the initial message of the simulator
        /// This can be helpful to renew the simulator message from file or database
        /// </summary>
        public override void LoadInitMessage(string message)
        {
            var type = ChesterCloudMessageAutoIdentifier.FindTypeByMessageStructure(message);
            if (type == null)
            {
                throw new Exception("Message structure not recognized");
            }
            if (type != typeof(T))
            {
                throw new Exception("Message structure not matching the simulator type");
            }

            //init the object based on found type
            var obj = Activator.CreateInstance(type);

            LastMessage = new SimulatedMessage<T>()
            {
                Id = Guid.NewGuid(),
                Timestamp = TimeHelpers.DateTimeToUnixTimestamp(DateTime.UtcNow),
                Message = obj as T
            };
        }
        /// <summary>
        /// Start Simulator main loop
        /// </summary>
        public override Task Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            return RunSimulation(_cancellationTokenSource.Token);
        }

        /// <summary>
        /// Stop Simulator main loop
        /// </summary>
        public override void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        private Task RunSimulation(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                _isRunning = true;
                OnSimulatorStarted?.Invoke(this, Id);

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay((int)Interval, cancellationToken);

                        var msgobj = Activator.CreateInstance(typeof(T));

                        if (msgobj == null)
                            throw new Exception("Failed to create message object");
                        var lastmsg = LastMessage?.Message ?? null;

                        BaseSimulator.GetSimulatedData(msgobj, lastmsg);

                        if (msgobj is ChesterCommonCloudMessage chesterCommonCloudMessage)
                        {
                            if (FwVersion != null)
                                chesterCommonCloudMessage.Attribute.FwVersion = FwVersion;
                            if (HwRevision != null)
                                chesterCommonCloudMessage.Attribute.HwRevision = HwRevision;
                            if (SerialNumber != null)
                                chesterCommonCloudMessage.Attribute.SerialNumber = SerialNumber;
                            if (IMEI != null)
                                chesterCommonCloudMessage.Network.Imei = (long)IMEI;
                            if (IMSI != null)
                                chesterCommonCloudMessage.Network.Imsi = (long)IMSI;
                        }

                        var msgid = Guid.NewGuid();
                        var timestamp = TimeHelpers.DateTimeToUnixTimestamp(DateTime.UtcNow);
                        _messages.TryAdd(msgid, new SimulatedMessage<T>()
                        {
                            Id = msgid,
                            Timestamp = timestamp,
                            Message = msgobj as T
                        });

                        if (_messages.TryGetValue(msgid, out var msgstored))
                            LastMessage = msgstored;

                        // if the _messages count is greater than MaximumStoredMessages, remove 10% of oldest messages
                        // this will decrease load to CPU because it will not happen in each cycle
                        var toTake = (int)((double)MaximumStoredMessages * 0.1);
                        if (MaximumStoredMessages < 5 || toTake < 1)
                            toTake = 1;

                        if (_messages.Count > MaximumStoredMessages)
                        {
                            var oldestMessages = _messages.OrderBy(x => x.Value.Timestamp)
                                                          .Take(toTake)
                                                          .Select(x => x.Key)
                                                          .ToList();

                            foreach (var key in oldestMessages)
                            {
                                _messages.TryRemove(key, out var _);
                            }
                        }

                        var msg = JsonSerializer.Serialize(msgobj);

                        OnDataGenerated?.Invoke(this, new ContinuousSimulatorEventArgs()
                        {
                            MessageId = msgid,
                            Message = msg,
                            SimulatorId = Id,
                            SimulatorName = Name,
                            Timestamp = timestamp
                        });
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"\n\nERROR>>>>>Exception in Simulator: {Name}, ID: {Id}, Exception Message: {ex.Message}\n\n");
                    }
                    
                }

                _isRunning = false;
                OnSimulatorEnded?.Invoke(this, Id);
                _cancellationTokenSource.Dispose(); 

            }, cancellationToken);
        }
    }
}

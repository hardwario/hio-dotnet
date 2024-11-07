using hio_dotnet.Common.Helpers;
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
        public override Guid Id { get; set; } = Guid.NewGuid();

        private bool _isRunning = false;
        public override bool IsRunning { get => _isRunning; }

        public override string Name { get; set; } = string.Empty;
        public override string Description { get; set; } = string.Empty;

        public override int MessagesCount => _messages.Count;

        public override long Interval { get; set; } = 5000;
        public SimulatedMessage<T>? LastMessage { get; set; }

        public override event EventHandler<ContinuousSimulatorEventArgs> OnDataGenerated;

        private CancellationTokenSource _cancellationTokenSource;

        private ConcurrentDictionary<Guid, SimulatedMessage<T>> _messages { get; set; } = new ConcurrentDictionary<Guid, SimulatedMessage<T>>();

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

        public override void SetInterval(long interval)
        {
            if (interval > 0)
                Interval = interval;
            else
                throw new Exception("Interval must be greater than 0");
        }

        public override SimulatedMessage<T>? GetLastMessage()
        {
            return LastMessage;
        }

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

        public override Task Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            return RunSimulation(_cancellationTokenSource.Token);
        }

        public override void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        private Task RunSimulation(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay((int)Interval, cancellationToken);

                    var msgobj = Activator.CreateInstance(typeof(T));

                    if (msgobj == null)
                        throw new Exception("Failed to create message object");
                    var lastmsg = LastMessage?.Message ?? null;

                    BaseSimulator.GetSimulatedData(msgobj, lastmsg);

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
            }, cancellationToken);
        }
    }
}

﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.DataSimulation
{
    public class SimulatorHandler
    {
        private ConcurrentDictionary<Guid, IContinuousSimulator> Simulators { get; set; } = new ConcurrentDictionary<Guid, IContinuousSimulator>();
        private ConcurrentDictionary<Guid, Task> SimulatorTasks { get; set; } = new ConcurrentDictionary<Guid, Task>();

        public int NumberOfRunningSimulators => SimulatorTasks.Count;
        /// <summary>
        /// Event which fires anytime when a new message is generated by any simulator
        /// </summary>
        public event EventHandler<ContinuousSimulatorEventArgs> OnDataGenerated;

        private void OnDataGeneratedHandler(object sender, ContinuousSimulatorEventArgs args)
        {
            OnDataGenerated?.Invoke(sender, args);
        }

        /// <summary>
        /// Add new simulator
        /// </summary>
        /// <param name="typeOfMessage">Define type of simulated message</param>
        /// <param name="interval">Interval in milliseconds</param>
        /// <param name="name">Name of simulator</param>
        /// <param name="description">Description of simulator</param>
        /// <param name="externalSoftwareId">If you use external software to forward the messages you can fill the external Id of this simulated device</param>
        /// <param name="externalSoftwareAccessToken">If you use external software to forward the messages you can fill the external access token of this simulated device</param>
        /// <param name="start">Start simulator automatically during the adding it</param>
        /// <param name="FwVersion">Static value of Firmware Version</param>
        /// <param name="HwRevision">Static value of Hardware Revision</param>
        /// <param name="SerialNumber">Static value of Serial Number</param>
        /// <param name="Imei">Static value of IMEI</param>
        /// <param name="Imsi">Static value of IMSI</param>
        /// <returns></returns>
        public Guid AddNewSimulator(Type typeOfMessage,
                                    long interval,
                                    string name,
                                    string description = "",
                                    string externalSoftwareId = "",
                                    string externalSoftwareAccessToken = "",
                                    bool start = true, 
                                    string? FwVersion = null,
                                    string? HwRevision = null,
                                    string? SerialNumber = null,
                                    long? Imei = null,
                                    long? Imsi = null)
        {
            if (typeOfMessage == null)
                return Guid.Empty;
            var gensim = Activator.CreateInstance(typeof(StandardContinuousSimulator<>).MakeGenericType(typeOfMessage));
            if (gensim == null)
                return Guid.Empty;
            var sim = (IContinuousSimulator)gensim;
            if (sim == null)
                return Guid.Empty;
            sim.Interval = interval;
            sim.Name = name;

            // load statics if exists
            if (!string.IsNullOrEmpty(FwVersion))
                sim.FwVersion = FwVersion;
            if (!string.IsNullOrEmpty(HwRevision))
                sim.HwRevision = HwRevision;
            if (!string.IsNullOrEmpty(SerialNumber))
                sim.SerialNumber = SerialNumber;
            if (Imei != null)
                sim.IMEI = Imei;
            if (Imsi != null)
                sim.IMSI = Imsi;

            sim.OnDataGenerated += OnDataGeneratedHandler;

            if (!string.IsNullOrEmpty(description))
                sim.Description = description;
            if (!string.IsNullOrEmpty(externalSoftwareId))
                sim.ExternalSoftwareId = externalSoftwareId;
            if (!string.IsNullOrEmpty(externalSoftwareAccessToken))
                sim.ExternalSoftwareAccessToken = externalSoftwareAccessToken;


            if (Simulators.TryAdd(sim.Id, sim))
            {
                if (start)
                {
                    var task = sim.Start();
                    SimulatorTasks.TryAdd(sim.Id, task);
                }
                return sim.Id;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Start simulator if it is not running already.
        /// This function will add Task to the internal SimulatorTasks dictionary.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool StartSimulator(Guid id)
        {
            if (Simulators.TryGetValue(id, out var sim))
            {
                if (sim.IsRunning)
                    return true;
                if (SimulatorTasks.ContainsKey(id))
                    return true;
                
                var task = sim.Start();
                return SimulatorTasks.TryAdd(id, task);
            }
            return false;
        }

        /// <summary>
        /// Stop simulator and remove Task from the internal SimulatorTasks dictionary.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        public async Task<bool> StopSimulator(Guid id)
        {
            if (Simulators.TryGetValue(id, out var sim))
            {
                if (!sim.IsRunning)
                {
                    //Console.WriteLine($"[INFO] Simulator {id} is not running.");
                    return true;
                }

                //Console.WriteLine($"[INFO] Stopping simulator {id}...");
                sim.Stop();

                if (SimulatorTasks.TryGetValue(id, out var task))
                {
                    try
                    {
                        //Console.WriteLine($"[INFO] Waiting for {id} task completed.");
                        await task;
                        Console.WriteLine($"[INFO] Simulator {id} Stopped.");
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine($"[INFO] Simulator {id} task was canceled.");
                    }
                    return SimulatorTasks.TryRemove(id, out _);
                }
            }
            return false;
        }

        /// <summary>
        /// Remove simulator from the dictionary.
        /// Function will stop simulator if it is running and it will remove the event handler.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> RemoveSimulator(Guid id)
        {
            if (Simulators.TryGetValue(id, out var sim))
            {
                await StopSimulator(id);
                sim.OnDataGenerated -= OnDataGeneratedHandler;

                return Simulators.TryRemove(id, out _);
            }

            return false;
        }

        /// <summary>
        /// Get specific simulator by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IContinuousSimulator? GetSimulator(Guid id)
        {
            if (Simulators.TryGetValue(id, out var sim))
                return sim;
            return null;
        }

        /// <summary>
        /// Get all simulators
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IContinuousSimulator> GetAllSimulators()
        {
            return Simulators.Values;
        }

        /// <summary>
        /// Stop All simulators
        /// </summary>
        public Task StopAllSimulators()
        {
            return Task.Run(async () =>
            {
                const int batchSize = 1000;
                
                while (SimulatorTasks.Count > 0)
                {
                    var simulatorsToStop = SimulatorTasks.Take(batchSize).Select(x => x.Key).ToList();

                    var stopTasks = simulatorsToStop.Select(id => StopSimulator(id));
                    await Task.WhenAll(stopTasks);
                }
            });
        }
        
        /// <summary>
        /// Await all simulators
        /// </summary>
        /// <returns></returns>
        public Task MonitorSimulations()
        {
            return Task.Run(async () =>
            {
                while (SimulatorTasks.Count > 0)
                {
                    // Await any non-completed tasks in the dictionary of Simulator Tasks
                    var completedTask = await Task.WhenAny(SimulatorTasks.Values);

                    // Remove completed task from the dictionary
                    var completedId = SimulatorTasks.FirstOrDefault(x => x.Value == completedTask).Key;
                    if (completedId != Guid.Empty)
                    {
                        //Console.WriteLine($"[INFO] Simulator with ID {completedId} has completed.");
                        SimulatorTasks.TryRemove(completedId, out _);
                    }
                }
            });
        }
    }
}
using hio_dotnet.Common.Config;
using hio_dotnet.HWDrivers.Enums;
using hio_dotnet.HWDrivers.Interfaces;
using hio_dotnet.HWDrivers.JLink;
using Org.BouncyCastle.Crypto.Engines;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.MCU
{
    public class MultiRTTClientBase
    {
        public RTTDriverType DriverType { get; set; } = RTTDriverType.None;
        public string Name { get; set; } = string.Empty;
        public int Channel { get; set; } = 0;
    }
    public class MultiRTTClientWSMessage : MultiRTTClientBase
    {
        public string Message { get; set; } = string.Empty;
    }
    public class MultiRTTClient : MultiRTTClientBase
    {
        public IRTTDriver? Driver { get; set; } = null;
        public IFWLoader? FWLoader { get; set; } = null;

    }

    public class MCUMultiRTTConsole
    {
        public MCUMultiRTTConsole(List<MultiRTTClientBase> clients, string mcutype, int speed, string consoleName, string jlinkSN = "0", uint rtt_address = 0)
        {

            if (!string.IsNullOrEmpty(consoleName))
                _consoleName = consoleName;

            if (!string.IsNullOrEmpty(mcutype))
                _mcuType = mcutype;
            else
                throw new ArgumentException("You must specify MCU Type.");

            if (speed > 0)
                _speed = speed;
            else
                throw new ArgumentException("Speed cannot be below or equal 0.");

            foreach(var  client in clients) {
                if (client.DriverType == RTTDriverType.None)
                {
                    throw new ArgumentException("You must specify the driver type.");
                }
                else if (client.DriverType == RTTDriverType.JLinkRTT)
                {
                    var cd = new JLinkDriver(mcutype, speed, jlinkSN, rtt_address);

                    Clients.TryAdd(client.Name, new MultiRTTClient() 
                    { 
                        DriverType = client.DriverType,
                        Driver = cd,
                        FWLoader = (IFWLoader)cd,
                        Name = client.Name, 
                        Channel = client.Channel 
                    });
                }
            }
        }
        /// <summary>
        /// List of all consoles lines received
        /// If it overflows MaxLines (default 200), it will remove the oldest lines
        /// </summary>
        public List<Tuple<string, MultiRTTClientBase>> ReceivedLines { get; set; } = new List<Tuple<string, MultiRTTClientBase>>();
        public int MaxLines { get; set; } = 200;
        public bool IsListening { get; set; } = false;
        public int Subscribers { get; set; } = 0;

        private string _consoleName = string.Empty;
        private string _mcuType = string.Empty;
        private int _speed = 4000;

        private ConcurrentDictionary<string, MultiRTTClient> Clients { get; set; } = new ConcurrentDictionary<string, MultiRTTClient>();

        /// <summary>
        /// Occurs when new not empty line from RTT channel has been received
        /// </summary>
        public event EventHandler<Tuple<string,MultiRTTClientBase>?> NewRTTMessageLineReceived;
        public event EventHandler<string> NewInternalCommandSent;


        /// <summary>
        /// Reconnect JLink RTT after firmware or MCU reset.
        /// </summary>
        public void ReconnectJLink()
        {
            foreach(var client in Clients.Values)
            {
                if (client.Driver != null)
                    client.Driver.ReconnectJLink();
            }
        }

        /// <summary>
        /// Start Listening Routine on RTT Read
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Running Task</returns>
        public Task StartListening(CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                Console.WriteLine($"Console {_consoleName} is starting listenning...");
                try
                {
                    IsListening = true;

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        foreach (var client in Clients.Values)
                        {
                            var message = client.Driver?.ReadRtt(client.Channel) ?? string.Empty;
                            if (!string.IsNullOrEmpty(message))
                            {
                                var lines = message.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var line in lines)
                                {
                                    var ln = line.Replace("\u001b[0m", "").Replace("\u001b[1m", "").Replace(" \u001b[m","").Replace("\u001b[1;32m","");
                                    var l = new Tuple<string, MultiRTTClientBase>(
                                        ln,
                                        new MultiRTTClientBase()
                                        {
                                            Name = client.Name,
                                            Channel = client.Channel,
                                            DriverType = client.DriverType
                                        });
                                    ReceivedLines.Add(l);
                                    NewRTTMessageLineReceived?.Invoke(this, l);
                                }
                            }
                        }

                        try
                        {
                            await Task.Delay(100, cancellationToken);
                        }
                        catch
                        {
                            Console.WriteLine($"Console {_consoleName} was canceled. Quiting Listening Loop.");
                            break;
                        }
                    }
                }
                finally
                {
                    IsListening = false;
                    CloseAll();
                }

            }, cancellationToken);
        }

        /// <summary>
        /// Send Command to the MCU by the channel name
        /// Line end \r\n is attached inside of driver automatically
        /// </summary>
        /// <param name="name">name of the channel</param>
        /// <param name="command">Command without line ending. Line end \r\n is attached inside of driver automatically</param>
        /// <returns>Running Task</returns>
        public Task SendCommand(string name, string command)
        {
            return Task.Run(() =>
            {
                if (Clients.TryGetValue(name, out var client))
                {
                    client.Driver?.WriteRtt(client.Channel, command);
                }
                else
                {
                    throw new ArgumentException("Client not found");
                }
            });
        }

        /// <summary>
        /// Send Command to the MCU by the channel number
        /// Line end \r\n is attached inside of driver automatically
        /// </summary>
        /// <param name="channel">RTT Channel number</param>
        /// <param name="command">Command without line ending. Line end \r\n is attached inside of driver automatically</param>
        /// <returns>Running Task</returns>
        public Task SendCommand(int channel, string command)
        {
            return Task.Run(() =>
            {
                var client = Clients.Values.FirstOrDefault(x => x.Channel == channel);
                if (client != null)
                {
                    client.Driver?.WriteRtt(client.Channel, command);
                }
                else
                {
                    throw new ArgumentException("Client not found");
                }
            });
        }

        /// <summary>
        /// Load firmware to the MCU with specified name of the channel
        /// Usually it does not matter what channel you will use because it internally uses same JLink Firmware driver to do the load of the firmware
        /// Filename must be specified to .hex file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool LoadFirmware(string name, string filename)
        {
            //return Task.Run(() =>
            //{
                if (Clients.TryGetValue(name, out var client))
                {
                    if (client.FWLoader != null)
                        return client.FWLoader.LoadFirmware(filename);
                }
                else
                {
                    throw new ArgumentException("Client not found");
                }
                return true;
            //});
        }

        /// <summary>
        /// Function will automatically iterate via device shell help and capture all commands and subcommands
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public async Task<List<ZephyrRTOSCommand>> LoadCommandsFromDeviceHelp(string parent, int channel)
        {
            var client = Clients.Values.FirstOrDefault(x => x.Channel == channel);
            if (client == null)
                return new List<ZephyrRTOSCommand>();
            
            if (client.Driver?.IsConnected ?? false)
            {
                var list = new List<ZephyrRTOSCommand>();
                var startindex = ReceivedLines.Where(l => l.Item2.Channel == channel).ToList().Count;
                if (parent == "")
                {
                    client.Driver?.WriteRtt(channel, "help");
                    NewInternalCommandSent?.Invoke(this, "> help");
                }
                else
                {
                    client.Driver?.WriteRtt(channel, parent + " -h");
                    NewInternalCommandSent?.Invoke(this, "> " + parent + " -h");
                }
                var count = 0;
                var lastcount = 0;
                var timeout = 10;
                lastcount = ReceivedLines.Where(l => l.Item2.Channel == channel).ToList().Count;
                while (true)
                {
                    await Task.Delay(50);
                    count = ReceivedLines.Where(l => l.Item2.Channel == channel).ToList().Count;

                    if (count == lastcount)
                        timeout--;

                    lastcount = count;
                    if (timeout == 0)
                        break;
                }

                var responses = ReceivedLines.Where(l => l.Item2.Channel == channel).Select(l => l.Item1).ToList().Skip(startindex).ToList();

                if (responses.Count > 1)
                {
                    for (var i = 0; i < responses.Count; i++)
                    {
                        if (i < responses.Count && i > 0)
                        {
                            var item = responses[i];
                            if (item.Contains("Subcommands:"))
                                continue;
                            if (i - 1 < 0)
                                continue;
                            var previtem = responses[i - 1];
                            if (previtem.Contains(" :") && !item.Contains(" :") && !item.Contains(" - "))
                            {
                                responses[i - 1] = previtem + item;
                                responses.RemoveAt(i);
                                i--;
                            }
                            else if (previtem.Contains(" - ") && !item.Contains(" - ") && !item.Contains(" :"))
                            {
                                responses[i - 1] = previtem + item;
                                responses.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
                                
                var subcommandsDetected = false;
                var prevR = string.Empty;
                foreach (var r in responses)
                {
                    if (!subcommandsDetected && r.Contains("Subcommands:"))
                    {
                        subcommandsDetected = true;
                    }
                    else
                    {
                        var split = r.Split(" :");
                        if (split != null && split.Length > 1)
                        {
                            var commandStart = split[0].TrimStart().TrimEnd();
                            if (list.Any(c => c.Command == commandStart))
                                continue;

                            var commandSubCommands = (await LoadCommandsFromDeviceHelp($"{parent} {commandStart}".TrimStart().TrimEnd(), channel)).ToList();

                            if (commandSubCommands.Count == 0)
                            {
                                var cmd = new ZephyrRTOSCommand()
                                {
                                    Command = commandStart,
                                    Description = split[1]
                                };
                                list.Add(cmd);
                            }
                            else
                            {
                                foreach (var csc in commandSubCommands)
                                {
                                    var des = csc.Description;
                                    if (string.IsNullOrEmpty(csc.Description))
                                        des = split[1];
                                    var cmd = new ZephyrRTOSCommand()
                                    {
                                        Command = $"{commandStart} {csc.Command}",
                                        Description = des
                                    };
                                    list.Add(cmd);
                                }
                            }
                        }
                        else
                        {
                            if (r.Contains("You can try to call commands with <-h> or <--help> parameter for more information."))
                                continue;

                            var skip = false;
                            if (!string.IsNullOrEmpty(prevR))
                            {
                                if (prevR.Replace(" :", " - ") == r)
                                    skip = true;
                            }

                            if (!skip)
                            {
                                var l = r.TrimStart().TrimEnd();
                                var c = string.Empty;
                                var d = string.Empty;
                                if (r.Contains(" - "))
                                {
                                    var sp = l.Split(" - ");
                                    if (sp != null && sp.Length > 1)
                                    {
                                        c = sp[0];
                                        d = sp[1];

                                        if (c == parent)
                                            continue;

                                        if (parent.Contains(" "))
                                        {
                                            var p = parent.Split(" ");

                                            if (c == p[p.Length - 1])
                                                continue;
                                        }
                                    }
                                }
                                else
                                {
                                    c = l;
                                }
                                var cmd = new ZephyrRTOSCommand()
                                {
                                    Command = c,
                                    Description = d
                                };
                                list.Add(cmd);
                            }
                        }
                    }
                    prevR = r;
                }
                return list;
            }
            return new List<ZephyrRTOSCommand>();
        }

        #region ClosingConnection
        /// <summary>
        /// Dispose client in case of closing app or destroying the class instance
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Close all connections
        /// </summary>
        public void CloseAll()
        {
            foreach (var client in Clients.Values)
                client.Driver?.Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var client in Clients.Values)
                    client.Driver?.Dispose();
            }
        }
        #endregion

    }

}

using hio_dotnet.HWDrivers.Enums;
using hio_dotnet.HWDrivers.Interfaces;
using hio_dotnet.HWDrivers.JLink;
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
    public class MultiRTTClient : MultiRTTClientBase
    {
        public IRTTDriver? Driver { get; set; } = null;
        public IFWLoader? FWLoader { get; set; } = null;

    }

    public class MCUMultiRTTConsole
    {
        public MCUMultiRTTConsole(List<MultiRTTClientBase> clients, string mcutype, int speed, string consoleName, string jlinkSN = "0")
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
                    var cd = new JLinkDriver(mcutype, speed, jlinkSN);

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

        private string _consoleName = string.Empty;
        private string _mcuType = string.Empty;
        private int _speed = 4000;

        private ConcurrentDictionary<string, MultiRTTClient> Clients { get; set; } = new ConcurrentDictionary<string, MultiRTTClient>();

        /// <summary>
        /// Occurs when new not empty line from RTT channel has been received
        /// </summary>
        public event EventHandler<Tuple<string,MultiRTTClientBase>?> NewRTTMessageLineReceived;


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
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        foreach (var client in Clients.Values)
                        {


                            string message = client.Driver?.ReadRtt(client.Channel) ?? string.Empty;

                            if (!string.IsNullOrEmpty(message))
                            {
                                var lines = message.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var line in lines)
                                {
                                    NewRTTMessageLineReceived?.Invoke(this, new Tuple<string, MultiRTTClientBase>(
                                        line,
                                        new MultiRTTClientBase()
                                        {
                                            Name = client.Name,
                                            Channel = client.Channel,
                                            DriverType = client.DriverType
                                        }));
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
        public Task LoadFirmware(string name, string filename)
        {
            return Task.Run(() =>
            {
                if (Clients.TryGetValue(name, out var client))
                {
                    if (client.FWLoader != null)
                        client.FWLoader.LoadFirmware(filename);
                }
                else
                {
                    throw new ArgumentException("Client not found");
                }
            });
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

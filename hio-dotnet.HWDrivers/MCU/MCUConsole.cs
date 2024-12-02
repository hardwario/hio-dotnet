using hio_dotnet.HWDrivers.Enums;
using hio_dotnet.HWDrivers.Interfaces;
using hio_dotnet.HWDrivers.JLink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.MCU
{
    public class MCUConsole : IDisposable
    {
        public MCUConsole(RTTDriverType driverType, string mcutype, int speed, int channel, string consoleName, string jlinkSN = "0")
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

            if (channel >= 0)
                _channel = channel;
            else
                throw new ArgumentException("Channel cannot be below 0.");

            if (driverType == RTTDriverType.None)
            {
                throw new ArgumentException("You must specify the driver type.");
            }
            else if (driverType == RTTDriverType.JLinkRTT)
            {
                _client = new JLinkDriver(mcutype, speed, jlinkSN);
                _fwloader = (IFWLoader)_client;
            }
        }

        private string _consoleName = string.Empty;
        private string _mcuType = string.Empty;
        private int _speed = 4000;
        private int _channel = 0;
        private IRTTDriver? _client = null;
        private IFWLoader? _fwloader = null;

        /// <summary>
        /// Occurs when new not empty line from RTT channel has been received
        /// </summary>
        public event EventHandler<string?> NewRTTMessageLineReceived;

        /// <summary>
        /// Reconnect JLink RTT after firmware or MCU reset.
        /// </summary>
        public void ReconnectJLink()
        {
            if (_client != null)
                _client.ReconnectJLink();
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
                Console.WriteLine($"Console {_consoleName} is starting listenning on channel {_channel}...");
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        string message = _client?.ReadRtt(_channel) ?? string.Empty;

                        if (!string.IsNullOrEmpty(message))
                        {
                            var lines = message.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var line in lines)
                            {
                                NewRTTMessageLineReceived?.Invoke(this, line);
                            }
                        }

                        try
                        {
                            await Task.Delay(100, cancellationToken);
                        }
                        catch
                        {
                            Console.WriteLine($"Console {_consoleName} was canceled. Quiting Listening Loop for channel {_channel}.");
                            break;
                        }

                    }
                }
                finally
                {
                    _client?.Close();
                }

            }, cancellationToken);
        }

        /// <summary>
        /// Send Command to the MCU
        /// Line end \r\n is attached inside of driver automatically
        /// </summary>
        /// <param name="command">Command without line ending. Line end \r\n is attached inside of driver automatically</param>
        /// <returns>Running Task</returns>
        public Task SendCommand(string command)
        {
            return Task.Run(() =>
            {
                _client?.WriteRtt(_channel, command);
            });
        }

        /// <summary>
        /// Load firmware to the MCU
        /// Filename must be specified to .hex file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Task LoadFirmware(string filename)
        {
            return Task.Run(() =>
            {
                if (_fwloader != null)
                    _fwloader.LoadFirmware(filename);
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client?.Dispose();
            }
        }
        #endregion
    }
}

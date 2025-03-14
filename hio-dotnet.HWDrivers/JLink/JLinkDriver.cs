using hio_dotnet.HWDrivers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.JLink
{
    public class JLinkDriver : IRTTDriver, IFWLoader, IDisposable
    {
        public JLinkDriver(string mcu = "nRF52840_xxAA", int speed = 4000, string serialNumber = "0", uint rtt_address = 0)
        {
            if (string.IsNullOrEmpty(mcu))
                throw new ArgumentException("JLink RTT Driver>> MCU type must be provided.");

            if (string.IsNullOrEmpty(serialNumber))
                serialNumber = "0";

            if (serialNumber != "0")
            {
                _serialNumber = serialNumber.Trim();
                JLink.EMU_SelectByUSBSN(Convert.ToInt32(serialNumber));
            }

            _mcu = mcu;
            _speed = speed;
            _rtt_address = rtt_address;

            CheckErrorCode(JLink.Open());

            SetMcuType(mcu);
            CheckErrorCode(JLink.TIF_Select(JLink.JLINKARM_TIF_SWD));
            JLink.SetSpeed(speed);

            CheckErrorCode(JLink.Connect());

            StartRtt();
            IsConnected = true;
            //CheckRttBufferCount();
        }

        public bool IsConnected { get; set; } = false;

        private string _serialNumber = "0";
        private string _mcu = "nRF52840_xxAA";
        private int _speed = 4000;
        private uint flashStartAddress = 0x100000;
        private uint _rtt_address = 0;

        private void CheckErrorCode(int errorCode)
        {
            var error = (JLinkErrorCodes)errorCode;
            if (error != JLinkErrorCodes.None)
            {
                if (error == JLinkErrorCodes.JLINK_ERR_VCC_FAILURE)
                {
                    throw new Exception("JLinkDriver>> Target power supply is not present.");
                }
                else if (error == JLinkErrorCodes.JLINK_ERR_NO_TARGET_DEVICE_SELECTED)
                {
                    throw new Exception("JLinkDriver>> No target device selected.");
                }
                else if (error == JLinkErrorCodes.JLINK_ERR_DLL_NOT_OPEN)
                {
                    throw new Exception("JLinkDriver>> JLink DLL is not open. Please check if the ./JLink/Driver/JLink_x64.dll is present.");
                }
                else if (error == JLinkErrorCodes.JLINK_ERR_NO_CPU_FOUND)
                {
                    throw new Exception("JLinkDriver>> No CPU found, please check the target type.");
                }
                else
                {
                    throw new Exception($"JLinkDriver>> Error Code: {error}");
                }
            }
        }

        private void LogMessage(string message)
        {
            Console.WriteLine($"[LOG] {message}");
        }

        /// <summary>
        /// Get all connected JLinks
        /// </summary>
        /// <returns></returns>
        public static JLINKARM_EMU_CONNECT_INFO[]? GetConnectedJLinks()
        {
            try
            {
                const int MaxDevices = 255;
                JLINKARM_EMU_CONNECT_INFO[] CInfo = new JLINKARM_EMU_CONNECT_INFO[MaxDevices];
                JLink.GetEmuList(MaxDevices, CInfo, MaxDevices);
                return CInfo;
            }
            catch(Exception ex)
            {
                Console.WriteLine("JLinkDriver>> Cannot get available JLinks. Error: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Reconnect to JLink
        /// It is helpful after the firmware update
        /// </summary>
        public void ReconnectJLink()
        {
            //JLink.Close();
            //System.Threading.Thread.Sleep(1000);

            CheckErrorCode(JLink.Open());
            SetMcuType(_mcu);
            CheckErrorCode(JLink.TIF_Select(JLink.JLINKARM_TIF_SWD));
            JLink.SetSpeed(_speed);

            CheckErrorCode(JLink.Connect());
            
            StartRtt();
            IsConnected = true;
        }

        private void SetMcuType(string mcuType)
        {
            string command = $"Device = {mcuType}";
            byte[] input = Encoding.ASCII.GetBytes(command);
            byte[] output = new byte[256];
            CheckErrorCode(JLink.ExecCommand(input, output, output.Length));
        }

        #region RTT

        private void CheckRttBufferCount()
        {
            uint numUpBuffers = 0;
            int result = JLink.RTTerminal_Control(JLink.JLINKARM_RTTERMINAL_CMD_GETNUMBUF, ref numUpBuffers);

            if (result >= 0 && numUpBuffers > 0)
            {
                LogMessage($"Number of RTT UP Buffers: {numUpBuffers}");
            }
            else
            {
                LogMessage("No RTT UP Buffers available.");
            }
        }

        private void CheckRttBufferDescription(uint bufferDir)
        {
            byte[] description = new byte[256];
            int result = JLink.RTTerminal_Control(JLink.JLINKARM_RTTERMINAL_CMD_GETDESC, ref bufferDir);
            if (result >= 0)
            {
                string desc = Encoding.ASCII.GetString(description);
                LogMessage($"RTT Buffer Description: {desc}");
            }
            else
            {
                LogMessage("Failed to get RTT buffer description.");
            }
        }

        private void StartRtt()
        {
            uint data = _rtt_address;
            LogMessage("Starting RTT...");
            int result = JLink.RTTerminal_Control(JLink.JLINKARM_RTTERMINAL_CMD_START, ref data);

            if (result < 0)
            {
                throw new Exception("RTT start failed.");
            }

            LogMessage("RTT started successfully.");
            System.Threading.Thread.Sleep(2000);

            var attempts = 5;
            while (attempts > 0)
            {
                var res = GetRttBufferCount(JLink.JLINKARM_RTTERMINAL_BUFFER_DIR_UP);
                if (res > 0)
                {
                    LogMessage("Up buffers found: " + res.ToString());
                    break;
                }
                else
                {
                    LogMessage("No Up buffers found: " + res.ToString());
                    attempts--;
                }
                System.Threading.Thread.Sleep(2000);
            }

            attempts = 5;
            while (attempts > 0)
            {
                var res = GetRttBufferCount(JLink.JLINKARM_RTTERMINAL_BUFFER_DIR_DOWN);
                if (res > 0)
                {
                    LogMessage("Down buffers found: " + res.ToString());
                    break;
                }
                else
                {
                    LogMessage("No Down buffers found: " + res.ToString());
                    attempts--;
                }
                System.Threading.Thread.Sleep(2000);
            }
        }

        private int GetRttBufferCount(uint direction)
        {
            return JLink.RTTerminal_Control(JLink.JLINKARM_RTTERMINAL_CMD_GETNUMBUF, ref direction);
        }

        public void WriteRtt(int bufferIndex, string message)
        {
            byte[] input = Encoding.ASCII.GetBytes(message + "\r\n");

            int result = JLink.RTTerminal_Write(bufferIndex, input, input.Length);
            if (result < 0)
            {
                throw new Exception($"RTT Write to buffer {bufferIndex} failed.");
            }
        }

        public string ReadRtt(int bufferIndex)
        {
            byte[] output = new byte[2048];

            int result = JLink.RTTerminal_Read(bufferIndex, output, output.Length);

            if (result > 0)
            {
                return Encoding.ASCII.GetString(output, 0, result);
            }
            return string.Empty;
        }
        #endregion

        #region FWLoading
        public bool LoadFirmware(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("FirmwareLoader>> Filename cannot be empty or null.");

            if (!filename.Contains(".hex"))
                throw new FormatException("FirmwareLoader>> Firmware File must be *.hex");

            if (!File.Exists(filename))
                throw new FileNotFoundException($"FirmwareLoader>> Cannot find the file: {filename}");

            if (!IsConnected)
                throw new Exception("FirmwareLoader>> Connection is not open.");
            
            try
            {
                LogMessage("FirmwareLoader>> Start Loading Firmware...");
                var res = JLink.DownloadFile(filename, flashStartAddress);
                if (res < 0)
                {
                    return false;
                    //throw new Exception("FirmwareLoader>> Cannot load the firmware.");
                }
                else
                {
                    LogMessage("FirmwareLoader>> Firmware File Loaded.");
                }

                LogMessage("FirmwareLoader>> Soft Restart MCU.");
                res = JLink.Reset();
                LogMessage("FirmwareLoader>> MCU Go.");
                res = JLink.Go();
            }
            catch (Exception ex)
            {
                LogMessage($"Loading failed: {ex.Message}");
                //throw;
                return false;
            }
            finally
            {
                Close();
            }
            return true;
        }

        #endregion

        #region ClosingConnection

        /// <summary>
        /// Close JLink connection
        /// </summary>
        public void Close()
        {
            if (IsConnected)
            {
                JLink.Close();
                IsConnected = false;
            }
        }

        /// <summary>
        /// Dispose JLink connection
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Threading;

namespace hio_dotnet.LibSerialPort
{
    /// <summary>
    /// A simple driver class that wraps libserialport functionality.
    /// </summary>
    public class LibSerialPortDriver : IDisposable
    {
        /// <summary>
        /// Event raised whenever new data is received.
        /// The byte[] argument contains the newly read data.
        /// </summary>
        public event EventHandler<byte[]> DataReceived;

        // Pointer to the native sp_port structure
        private IntPtr _portPtr = IntPtr.Zero;

        // A separate thread for continuously reading incoming data
        private Thread _readThread;
        private bool _readThreadRunning;
        private bool _quitLoop;

        private object _lock = new object(); // for basic thread-safety

        /// <summary>
        /// Retrieves a list of available serial ports on the system.
        /// </summary>
        public static string[] GetPortNames()
        {
            List<string> portNames = new List<string>();

            // sp_list_ports(struct sp_port ***list_ptr)
            int result = LibSerialPort.sp_list_ports(out IntPtr listPtr);
            if (result != (int)SpReturn.SP_OK || listPtr == IntPtr.Zero)
            {
                // If no ports found or error, we can return an empty list
                return portNames.ToArray();
            }

            try
            {
                // 'listPtr' is a pointer to an array of pointers to sp_port.
                // We have to iterate until we hit a NULL pointer.
                int index = 0;
                while (true)
                {
                    // Calculate pointer offset
                    IntPtr currentPortPtr = System.Runtime.InteropServices.Marshal.ReadIntPtr(listPtr, index * IntPtr.Size);
                    if (currentPortPtr == IntPtr.Zero)
                        break;

                    // Get the port name
                    string name = LibSerialPort.sp_get_port_name_private(currentPortPtr);
                    if (!string.IsNullOrEmpty(name))
                    {
                        portNames.Add(name);
                    }

                    index++;
                }
            }
            finally
            {
                // Free the port list
                LibSerialPort.sp_free_port_list(listPtr);
            }
            return portNames.ToArray();
        }

        /// <summary>
        /// Opens the specified port with basic parameters (baudrate, dataBits, parity, stopBits).
        /// Optionally you can add more config if needed (flow control etc.).
        /// </summary>
        public void OpenPort(string portName, int baudRate, int dataBits, SpParity parity, SpStopBits stopBits)
        {
            lock (_lock)
            {
                if (_portPtr != IntPtr.Zero)
                    throw new InvalidOperationException("Port already opened.");

                // sp_get_port_by_name
                int res = LibSerialPort.sp_get_port_by_name(portName, out _portPtr);
                if (res != (int)SpReturn.SP_OK || _portPtr == IntPtr.Zero)
                {
                    throw new Exception($"Could not get port '{portName}', error = {res}");
                }

                // sp_open
                res = LibSerialPort.sp_open(_portPtr, SpMode.SP_MODE_READ_WRITE);
                if (res != (int)SpReturn.SP_OK)
                {
                    Close();
                    throw new Exception($"Could not open port '{portName}', error = {res}");
                }

                // Set basic config: baudrate, bits, parity, stopbits
                res = LibSerialPort.sp_set_baudrate(_portPtr, baudRate);
                CheckAndThrow(res, "sp_set_baudrate");

                res = LibSerialPort.sp_set_bits(_portPtr, dataBits);
                CheckAndThrow(res, "sp_set_bits");

                res = LibSerialPort.sp_set_parity(_portPtr, parity);
                CheckAndThrow(res, "sp_set_parity");

                res = LibSerialPort.sp_set_stopbits_ext(_portPtr, stopBits);
                CheckAndThrow(res, "sp_set_stopbits");

                // Start the background thread for reading
                _readThreadRunning = true;
                _readThread = new Thread(ReadLoop)
                {
                    IsBackground = true
                };
                _readThread.Start();
                
            }
        }

        /// <summary>
        /// Sets the RTS (Request To Send) line to on/off (or other advanced modes).
        /// For on/off, we use SP_RTS_ON or SP_RTS_OFF from the SpRts enum.
        /// </summary>
        public void SetRTS(bool enable)
        {
            lock (_lock)
            {
                if (_portPtr == IntPtr.Zero)
                    throw new InvalidOperationException("Port not open.");

                // sp_set_rts(struct sp_port *port, enum sp_rts rts)
                SpRts mode = enable ? SpRts.SP_RTS_ON : SpRts.SP_RTS_OFF;
                int res = LibSerialPort.sp_set_rts(_portPtr, mode);
                CheckAndThrow(res, "sp_set_rts");
            }
        }

        /// <summary>
        /// Sets the DTR (Data Terminal Ready) line to on/off.
        /// </summary>
        public void SetDTR(bool enable)
        {
            lock (_lock)
            {
                if (_portPtr == IntPtr.Zero)
                    throw new InvalidOperationException("Port not open.");

                SpDtr mode = enable ? SpDtr.SP_DTR_ON : SpDtr.SP_DTR_OFF;
                int res = LibSerialPort.sp_set_dtr(_portPtr, mode);
                CheckAndThrow(res, "sp_set_dtr");
            }
        }

        /// <summary>
        /// Sends data to the serial port.
        /// </summary>
        public void Write(byte[] data)
        {
            if (data == null || data.Length == 0)
                return;

            lock (_lock)
            {
                if (_portPtr == IntPtr.Zero)
                    throw new InvalidOperationException("Port not open.");

                // sp_blocking_write
                IntPtr length = (IntPtr)data.Length;
                int res = LibSerialPort.sp_blocking_write(_portPtr, data, length, 500 /*ms timeout*/);
                if (res < 0)
                {
                    throw new Exception($"sp_blocking_write failed, code = {res}, msg = {LibSerialPort.sp_last_error_message()}");
                }
            }
        }

        /// <summary>
        /// Closes the serial port and stops the reading thread.
        /// </summary>
        public void Close()
        {
            lock (_lock)
            {
                if (_portPtr != IntPtr.Zero)
                {
                    // stop reading
                    _readThreadRunning = false;
                }
            }

            // Wait for the thread to finish outside the lock to avoid deadlocks
            if (_readThread != null && _readThread.IsAlive)
            {
                _readThread.Join();
                _readThread = null;
            }

            lock (_lock)
            {
                if (_portPtr != IntPtr.Zero)
                {
                    LibSerialPort.sp_close(_portPtr);
                    LibSerialPort.sp_free_port(_portPtr);
                    _portPtr = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// The main read loop that runs in a separate thread. 
        /// It waits for data and raises DataReceived event.
        /// </summary>
        private void ReadLoop()
        {
            const int BUFFER_SIZE = 1024;
            byte[] buffer = new byte[BUFFER_SIZE];

            while (true)
            {
                // Check if we should stop
                if (_quitLoop)
                    return;

                lock (_lock)
                {
                    if (!_readThreadRunning || _portPtr == IntPtr.Zero)
                    {
                        // Port closed or stop requested
                        return;
                    }
                }

                // Try to read using a short timeout
                IntPtr size = (IntPtr)buffer.Length;
                int bytesRead = LibSerialPort.sp_blocking_read(_portPtr, buffer, size, 100 /* ms timeout */);
                if (bytesRead < 0)
                {
                    //small dealy to release main thread
                    Thread.Sleep(5);
                    // Error or no data within the timeout
                    // Could check sp_last_error_code() or message
                    // We'll just continue
                    continue;
                }
                else if (bytesRead == 0)
                {
                    //small dealy to release main thread
                    Thread.Sleep(5);
                    // No data available in this timeframe, try again
                    continue;
                }
                else
                {
                    // We got some data
                    byte[] dataCopy = new byte[bytesRead];
                    Array.Copy(buffer, dataCopy, bytesRead);

                    // Raise the event
                    DataReceived?.Invoke(this, dataCopy);
                }
                
            }
        }

        /// <summary>
        /// Helper method to throw an exception if sp_ function didn't return SP_OK
        /// </summary>
        private void CheckAndThrow(int result, string functionName)
        {
            if (result != (int)SpReturn.SP_OK)
            {
                string msg = LibSerialPort.sp_last_error_message();
                throw new Exception($"{functionName} failed with code={result}, msg={msg}");
            }
        }

        /// <summary>
        /// Dispose pattern to ensure the port is closed if needed.
        /// </summary>
        public void Dispose()
        {
            lock (_lock)
            {
                _quitLoop = true;
            }
            Close();
        }
    }
}

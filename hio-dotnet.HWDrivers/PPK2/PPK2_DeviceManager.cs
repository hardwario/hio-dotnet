using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using hio_dotnet.LibSerialPort;

namespace hio_dotnet.HWDrivers.PPK2
{
    public class PPK2_DeviceManager
    {
        private static readonly TraceSource Logger = new TraceSource("PPK2DeviceManager");

        /// <summary>
        /// Lists all available PPK2 devices with their COM ports and serial numbers.
        /// </summary>
        /// <returns>A list of tuples containing COM port and serial number of each connected PPK2 device.</returns>
        public static List<(string PortName, string SerialNumber)> ListAvailablePPK2Devices()
        {
            List<(string PortName, string SerialNumber)> devices = new List<(string PortName, string SerialNumber)>();

            try
            {
                // Get a list of all connected COM ports
                string[] portNames = LibSerialPortDriver.GetPortNames();

#if WINDOWS
                // Windows-specific implementation using System.Management
                try
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'"))
                    {
                        foreach (var obj in searcher.Get())
                        {
                            string name = obj["Name"]?.ToString();
                            string deviceID = obj["DeviceID"]?.ToString();

                            if (name != null && name.Contains("nRF Connect USB CDC ACM"))
                            {
                                var comPort = ExtractComPort(name);

                                if (portNames.Contains(comPort))
                                {
                                    string serialNumber = ExtractSerialNumberFromDeviceID(deviceID);
                                    devices.Add((comPort, serialNumber));
                                    Logger.TraceInformation($"Detected PPK2 device on {comPort} with serial number {serialNumber}");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.TraceEvent(TraceEventType.Error, 0, $"Error while retrieving PPK2 devices on Windows: {ex.Message}");
                }

#elif LINUX
                // Linux-specific implementation using udevadm
                try
                {
                    foreach (string portName in portNames)
                    {
                        // Use udevadm to get device details
                        string output = ExecuteBashCommand($"udevadm info -q property -n {portName} | grep -E 'ID_MODEL=PPK2|ID_SERIAL_SHORT='");

                        if (!string.IsNullOrEmpty(output) && output.Contains("ID_MODEL=PPK2"))
                        {
                            // Extract the serial number
                            string serialNumber = ExtractLinuxSerialNumber(output);
                            devices.Add((portName, serialNumber));
                            Logger.TraceInformation($"Detected PPK2 device on {portName} with serial number {serialNumber}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.TraceEvent(TraceEventType.Error, 0, $"Error while retrieving PPK2 devices on Linux: {ex.Message}");
                }

#elif OSX
                // macOS-specific implementation using ioreg
                try
                {
                    foreach (string portName in portNames)
                    {
                        // Use ioreg to get device details
                        string output = ExecuteBashCommand($"ioreg -r -c IOSerialBSDClient | grep {portName}");

                        if (!string.IsNullOrEmpty(output))
                        {
                            string serialNumber = ExtractSerialNumberFromIoregOutput(output);
                            devices.Add((portName, serialNumber));
                            Logger.TraceInformation($"Detected PPK2 device on {portName} with serial number {serialNumber}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.TraceEvent(TraceEventType.Error, 0, $"Error retrieving serial number for port {portName} on macOS: {ex.Message}");
                }

#else
                // Unsupported platform
                /*
                Logger.TraceEvent(TraceEventType.Warning, 0, "Running on an unsupported platform.");
                foreach (string portName in portNames)
                {
                    devices.Add((portName, "UnknownSerial"));
                }
                */
#endif
            }
            catch (Exception ex)
            {
                Logger.TraceEvent(TraceEventType.Error, 0, $"Unhandled error while listing PPK2 devices: {ex.Message}");
            }

            return devices;
        }

        // Extract COM port from device name (Windows)
        private static string ExtractComPort(string deviceName)
        {
            int startIndex = deviceName.LastIndexOf("(COM");
            if (startIndex > 0)
            {
                int endIndex = deviceName.LastIndexOf(")");
                return deviceName.Substring(startIndex + 1, endIndex - startIndex - 1);
            }
            return string.Empty;
        }

        // Extract serial number from device ID (Windows)
        private static string ExtractSerialNumberFromDeviceID(string deviceID)
        {
            if (string.IsNullOrEmpty(deviceID))
                return "UnknownSerial";

            try
            {
                var parts = deviceID.Split('\\');
                if (parts.Length > 2)
                {
                    var serialPart = parts[2];
                    if (serialPart.Contains("&"))
                    {
                        var serialSegments = serialPart.Split('&');
                        return serialSegments[^1];
                    }
                    return serialPart;
                }
            }
            catch (Exception ex)
            {
                Logger.TraceEvent(TraceEventType.Error, 0, $"Error extracting serial number from DeviceID: {ex.Message}");
            }
            return "UnknownSerial";
        }

        // Execute bash command (Linux/macOS)
        private static string ExecuteBashCommand(string command)
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"" + command + "\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using (var process = new Process())
                {
                    process.StartInfo = processInfo;
                    process.Start();
                    string result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return result.Trim();
                }
            }
            catch (Exception ex)
            {
                Logger.TraceEvent(TraceEventType.Error, 0, $"Error executing bash command '{command}': {ex.Message}");
            }
            return string.Empty;
        }

        // Extract serial number from udevadm output (Linux)
        private static string ExtractLinuxSerialNumber(string output)
        {
            try
            {
                string[] lines = output.Split('\n');
                foreach (var line in lines)
                {
                    if (line.StartsWith("ID_SERIAL_SHORT="))
                    {
                        return line.Split('=')[1].Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.TraceEvent(TraceEventType.Error, 0, $"Error extracting serial number from udevadm output: {ex.Message}");
            }
            return "UnknownSerial";
        }

        // Extract serial number from ioreg output (macOS)
        private static string ExtractSerialNumberFromIoregOutput(string ioregOutput)
        {
            try
            {
                string[] lines = ioregOutput.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Contains("IODialinDevice"))
                    {
                        var parts = line.Split('=');
                        if (parts.Length > 1)
                        {
                            return parts[1].Trim().Trim('"');
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.TraceEvent(TraceEventType.Error, 0, $"Error extracting serial number from ioreg output: {ex.Message}");
            }
            return "UnknownSerial";
        }
    }
}
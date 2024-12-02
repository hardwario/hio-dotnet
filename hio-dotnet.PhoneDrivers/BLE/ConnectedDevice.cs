using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.PhoneDrivers.BLE
{
    public class ConnectedDevice
    {
        public string ProductName { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public string HardwareVariant { get; set; } = string.Empty;
        public string HardwareRevision { get; set; } = string.Empty;
        public string FirmwareName { get; set; } = string.Empty;
        public string FirmwareVersion { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string ClaimToken { get; set; } = string.Empty;
        public string BluetoothAddress { get; set; } = string.Empty;
        public string BluetoothKey { get; set; } = string.Empty;
    }
}

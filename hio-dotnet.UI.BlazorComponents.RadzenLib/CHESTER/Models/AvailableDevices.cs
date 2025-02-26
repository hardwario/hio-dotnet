using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.CHESTER.Models
{
    public class AvailableDevice
    {
        public AvailableDevice() { }
        public AvailableDevice(string name, string mcu, int speed, uint rttaddress)
        {
            if (name != null)
                Name = name;
            if (mcu != null)
                MCU = mcu;
            if (speed > 0)
                Speed = speed;
            RTTAddress = rttaddress;
        }
        public string Name { get; set; } = "CHESTER";
        public string MCU { get; set; } = "nRF52840_xxAA";
        public int Speed { get; set; } = 4000;
        public uint RTTAddress { get; set; } = 0;
    }

    public static class AvailableDevices
    {
        public static List<AvailableDevice> Devices = new List<AvailableDevice>()
        {
            new AvailableDevice("CHESTER", "nRF52840_xxAA", 4000, 0),
            new AvailableDevice("STICKER", "STM32WLE5CC", 4000, 0x20000800)
        };
    }
}

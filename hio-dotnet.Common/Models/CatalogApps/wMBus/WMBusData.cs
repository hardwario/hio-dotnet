using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.wMBus
{
    public class WMBusData
    {
        [JsonPropertyName("cycle")]
        public int Cycle { get; set; } = 0;
        [JsonPropertyName("devices")]
        public int Devices { get; set; } = 0;

        [JsonPropertyName("part")]
        public int Part { get; set; } = 0;

        [JsonPropertyName("received")]
        public int Received { get; set; } = 0;

        [JsonPropertyName("scan_time")]
        public int ScanTime { get; set; } = 0;

        [JsonPropertyName("packets")]
        public List<WMBusPacket> Packets { get; set; } = new List<WMBusPacket>();
    }
}

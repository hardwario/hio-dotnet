using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class ChesterSystem
    {
        [JsonPropertyName("uptime")]
        public long? Uptime { get; set; } = 0;

        [JsonPropertyName("voltage_rest")]
        public double? VoltageRest { get; set; } = 0.0;

        [JsonPropertyName("voltage_load")]
        public double? VoltageLoad { get; set; } = 0.0;

        [JsonPropertyName("current_load")]
        public int? CurrentLoad { get; set; } = 0;
    }

}

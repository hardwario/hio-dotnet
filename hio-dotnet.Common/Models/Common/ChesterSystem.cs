using hio_dotnet.Common.Models.DataSimulation;
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
        [SimulationAttribute(false, 0, 1000000, true, true, 0.01)]
        [JsonPropertyName("uptime")]
        public long? Uptime { get; set; } = 0;

        [SimulationAttribute(false, 3.0, 4.2, true, false, 0.001)]
        [JsonPropertyName("voltage_rest")]
        public double? VoltageRest { get; set; } = 0.0;

        [SimulationAttribute(false, 3.0, 4.2, true, false, 0.001)]
        [JsonPropertyName("voltage_load")]
        public double? VoltageLoad { get; set; } = 0.0;

        [SimulationAttribute(false, 0.1, 1.0, true, false, 0.001)]
        [JsonPropertyName("current_load")]
        public int? CurrentLoad { get; set; } = 0;
    }

}

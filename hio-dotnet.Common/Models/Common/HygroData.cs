using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class HygroData : TimestampState
    {
        [SimulationAttribute(false, 0, 100.0, true, false, 0.05)]
        [JsonPropertyName("humidity")]
        public double Humidity { get; set; } = 0.0;

        [SimulationAttribute(false, 0, 30, true, false, 0.05)]
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.0;
    }
}

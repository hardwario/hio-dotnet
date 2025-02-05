using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class Measurement : TimestampState
    {
        [SimulationAttribute(false)]
        [JsonPropertyName("min")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Min { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("max")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Max { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("avg")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Avg { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("mdn")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Mdn { get; set; } = 0.0;
    }
}

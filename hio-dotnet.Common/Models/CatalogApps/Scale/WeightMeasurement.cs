using hio_dotnet.Common.Models.Common;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Scale
{
    public class WeightMeasurement : TimestampState
    {
        [SimulationAttribute(false, -10.0, 10.0, true, true, 0.1)]
        [JsonPropertyName("raw_result_a1")]
        public int? A1 { get; set; }

        [SimulationAttribute(false, -210.0, -180.0, true, true, 0.05)]
        [JsonPropertyName("raw_result_a2")]
        public int? A2 { get; set; }

        [SimulationAttribute(false, -10.0, 10.0, true, true, 0.1)]
        [JsonPropertyName("raw_result_b1")]
        public int? B1 { get; set; }

        [SimulationAttribute(false, -210.0, -180.0, true, true, 0.05)]
        [JsonPropertyName("raw_result_b2")]
        public int? B2 { get; set; }
    }
}

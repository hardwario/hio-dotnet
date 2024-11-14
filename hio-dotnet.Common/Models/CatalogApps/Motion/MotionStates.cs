using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Motion
{
    public class MotionStates
    {
        [SimulationAttribute(false, 0.0, 10000.0, true, true, 0.005)]
        [JsonPropertyName("count_left")]
        public int? CountLeft { get; set; }

        [SimulationAttribute(false, 0.0, 10000.0, true, true, 0.005)]
        [JsonPropertyName("count_right")]
        public int? CountRight { get; set; }
    }
}

using hio_dotnet.Common.Models.Common;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Push
{
    public class PushButtonsStates
    {
        [SimulationAttribute(true)]
        [JsonPropertyName("button")]
        public string? Button { get; set; }

        [SimulationAttribute(false, 0.0, 10000.0, true, false, 0.005)]
        [JsonPropertyName("count_click")]
        public int CountClick { get; set; } = 0;

        [SimulationAttribute(false, 0.0, 10000.0, true, false, 0.005)]
        [JsonPropertyName("count_hold")]
        public int CountHold { get; set; } = 0;

        [JsonPropertyName("events")]
        public List<ChesterBaseEvent>? Events { get; set; }
    }
}

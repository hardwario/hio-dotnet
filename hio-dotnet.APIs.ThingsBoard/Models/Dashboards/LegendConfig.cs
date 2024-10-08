using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class LegendConfig
    {
        [JsonPropertyName("direction")]
        public string? Direction { get; set; } = "column";

        [JsonPropertyName("position")]
        public string? Position { get; set; } = "top";

        [JsonPropertyName("sortDataKeys")]
        public bool SortDataKeys { get; set; } = false;

        [JsonPropertyName("showMin")]
        public bool ShowMin { get; set; } = false;

        [JsonPropertyName("showMax")]
        public bool ShowMax { get; set; } = false;

        [JsonPropertyName("showAvg")]
        public bool ShowAvg { get; set; } = true;

        [JsonPropertyName("showTotal")]
        public bool ShowTotal { get; set; } = false;

        [JsonPropertyName("showLatest")]
        public bool ShowLatest { get; set; } = false;
    }
}

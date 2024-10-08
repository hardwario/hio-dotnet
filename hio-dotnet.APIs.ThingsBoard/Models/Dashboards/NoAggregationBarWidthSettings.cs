using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class NoAggregationBarWidthSettings
    {
        [JsonPropertyName("strategy")]
        public string? Strategy { get; set; } = "group";

        [JsonPropertyName("groupWidth")]
        public BarWidthSettings GroupWidth { get; set; } = new BarWidthSettings();

        [JsonPropertyName("barWidth")]
        public BarWidthSettings BarWidth { get; set; } = new BarWidthSettings();
    }
}

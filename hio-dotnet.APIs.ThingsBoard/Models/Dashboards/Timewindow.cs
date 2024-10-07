using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class Timewindow
    {
        [JsonPropertyName("hideInterval")]
        public bool HideInterval { get; set; } = false;

        [JsonPropertyName("realtime")]
        public Realtime Realtime { get; set; } = new Realtime();

        [JsonPropertyName("aggregation")]
        public Aggregation Aggregation { get; set; } = new Aggregation();
    }
}

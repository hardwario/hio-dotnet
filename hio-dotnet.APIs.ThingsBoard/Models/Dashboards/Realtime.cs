using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class Realtime
    {
        [JsonPropertyName("realtimeType")]
        public int RealtimeType { get; set; } = 0;

        [JsonPropertyName("timewindowMs")]
        public long TimewindowMs { get; set; } = 604800000;
    }
}

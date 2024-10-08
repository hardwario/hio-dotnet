using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class FixedTimewindow
    {
        [JsonPropertyName("startTimeMs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? StartTimeMs { get; set; }

        [JsonPropertyName("endTimeMs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? EndTimeMs { get; set; }
    }
}

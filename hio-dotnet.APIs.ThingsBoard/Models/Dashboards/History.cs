using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class History
    {
        [JsonPropertyName("historyType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? HistoryType { get; set; }

        [JsonPropertyName("interval")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Interval { get; set; }

        [JsonPropertyName("timewindowMs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? TimewindowMs { get; set; }

        [JsonPropertyName("fixedTimewindow")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FixedTimewindow? FixedTimewindow { get; set; }

        [JsonPropertyName("quickInterval")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? QuickInterval { get; set; }
    }
}

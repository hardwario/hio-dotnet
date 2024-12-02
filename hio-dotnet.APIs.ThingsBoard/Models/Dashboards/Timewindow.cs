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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? HideInterval { get; set; }

        [JsonPropertyName("hideLastInterval")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? HideLastInterval { get; set; }

        [JsonPropertyName("hideQuickInterval")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? HideQuickInterval { get; set; }

        [JsonPropertyName("hideAggregation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? HideAggregation { get; set; }

        [JsonPropertyName("hideAggInterval")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? HideAggInterval { get; set; }

        [JsonPropertyName("hideTimezone")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? HideTimezone { get; set; }

        [JsonPropertyName("selectedTab")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? SelectedTab { get; set; }

        [JsonPropertyName("realtime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Realtime? Realtime { get; set; }

        [JsonPropertyName("aggregation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Aggregation? Aggregation { get; set; }

        [JsonPropertyName("history")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public History? History { get; set; }

        [JsonPropertyName("timezone")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Timezone { get; set; }
    }
}

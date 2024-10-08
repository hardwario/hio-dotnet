using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class Animation
    {
        [JsonPropertyName("animation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? EnableAnimation { get; set; }

        [JsonPropertyName("animationThreshold")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? AnimationThreshold { get; set; }

        [JsonPropertyName("animationDuration")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? AnimationDuration { get; set; }

        [JsonPropertyName("animationEasing")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AnimationEasing { get; set; }

        [JsonPropertyName("animationDelay")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? AnimationDelay { get; set; }

        [JsonPropertyName("animationDurationUpdate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? AnimationDurationUpdate { get; set; }

        [JsonPropertyName("animationEasingUpdate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AnimationEasingUpdate { get; set; }

        [JsonPropertyName("animationDelayUpdate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? AnimationDelayUpdate { get; set; }
    }
}

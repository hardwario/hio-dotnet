using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class BarSettings
    {
        [JsonPropertyName("showBorder")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowBorder { get; set; }

        [JsonPropertyName("borderWidth")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BorderWidth { get; set; }

        [JsonPropertyName("borderRadius")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BorderRadius { get; set; }

        [JsonPropertyName("showLabel")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowLabel { get; set; }

        [JsonPropertyName("labelPosition")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LabelPosition { get; set; }

        [JsonPropertyName("labelFont")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FontSettings? LabelFont { get; set; }

        [JsonPropertyName("labelColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LabelColor { get; set; }

        [JsonPropertyName("enableLabelBackground")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? EnableLabelBackground { get; set; }

        [JsonPropertyName("labelBackground")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LabelBackground { get; set; }

        [JsonPropertyName("backgroundSettings")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public BackgroundSettings? BackgroundSettings { get; set; }
    }
}

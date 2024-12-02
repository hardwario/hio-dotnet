using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class AxisDetails
    {
        [JsonPropertyName("units")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Units { get; set; }

        [JsonPropertyName("decimals")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Decimals { get; set; }

        [JsonPropertyName("show")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Show { get; set; }

        [JsonPropertyName("label")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Label { get; set; }

        [JsonPropertyName("labelFont")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FontSettings? LabelFont { get; set; }

        [JsonPropertyName("labelColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LabelColor { get; set; }

        [JsonPropertyName("position")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Position { get; set; }

        [JsonPropertyName("showTickLabels")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowTickLabels { get; set; }

        [JsonPropertyName("tickLabelFont")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FontSettings? TickLabelFont { get; set; }

        [JsonPropertyName("tickLabelColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TickLabelColor { get; set; }

        [JsonPropertyName("showTicks")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowTicks { get; set; }

        [JsonPropertyName("ticksColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TicksColor { get; set; }

        [JsonPropertyName("showLine")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowLine { get; set; }

        [JsonPropertyName("lineColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LineColor { get; set; }

        [JsonPropertyName("showSplitLines")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowSplitLines { get; set; }

        [JsonPropertyName("splitLinesColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SplitLinesColor { get; set; }

        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Id { get; set; }

        [JsonPropertyName("order")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Order { get; set; }
    }
}

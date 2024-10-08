using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class AxisSettings
    {
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

    }
}

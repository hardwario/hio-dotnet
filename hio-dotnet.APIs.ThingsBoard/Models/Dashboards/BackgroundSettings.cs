using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class BackgroundSettings
    {
        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Type { get; set; }

        [JsonPropertyName("opacity")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Opacity { get; set; }

        [JsonPropertyName("gradient")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public GradientSettings? Gradient { get; set; }
    }
}

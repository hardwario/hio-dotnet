using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class FillAreaSettings
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; } = "none";

        [JsonPropertyName("opacity")]
        public double Opacity { get; set; } = 0.4;

        [JsonPropertyName("gradient")]
        public GradientSettings Gradient { get; set; } = new GradientSettings();
    }
}

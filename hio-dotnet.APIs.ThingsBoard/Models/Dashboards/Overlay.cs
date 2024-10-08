using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class Overlay
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = false;

        [JsonPropertyName("color")]
        public string? Color { get; set; } = "rgba(255,255,255,0.72)";

        [JsonPropertyName("blur")]
        public int Blur { get; set; } = 3;
    }
}

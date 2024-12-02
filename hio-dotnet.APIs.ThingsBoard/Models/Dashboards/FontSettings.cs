using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class FontSettings
    {
        [JsonPropertyName("family")]
        public string? Family { get; set; } = "Roboto";

        [JsonPropertyName("size")]
        public int Size { get; set; } = 11;

        [JsonPropertyName("sizeUnit")]
        public string? SizeUnit { get; set; } = "px";

        [JsonPropertyName("style")]
        public string? Style { get; set; } = "normal";

        [JsonPropertyName("weight")]
        public string? Weight { get; set; } = "400";

        [JsonPropertyName("lineHeight")]
        public string? LineHeight { get; set; } = "1";
    }
}

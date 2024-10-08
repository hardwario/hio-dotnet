using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class TimewindowStyle
    {
        [JsonPropertyName("showIcon")]
        public bool ShowIcon { get; set; } = false;

        [JsonPropertyName("iconSize")]
        public string? IconSize { get; set; } = "24px";

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        [JsonPropertyName("iconPosition")]
        public string? IconPosition { get; set; } = "left";

        [JsonPropertyName("font")]
        public FontSettings Font { get; set; } = new FontSettings();

        [JsonPropertyName("color")]
        public string? Color { get; set; } = "rgba(0, 0, 0, 0.38)";

        [JsonPropertyName("displayTypePrefix")]
        public bool DisplayTypePrefix { get; set; } = true;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class TooltipDateFormat
    {
        [JsonPropertyName("format")]
        public string? Format { get; set; }

        [JsonPropertyName("lastUpdateAgo")]
        public bool LastUpdateAgo { get; set; } = false;

        [JsonPropertyName("custom")]
        public bool Custom { get; set; } = false;

        [JsonPropertyName("auto")]
        public bool Auto { get; set; } = true;

        [JsonPropertyName("autoDateFormatSettings")]
        public Dictionary<string, object>? AutoDateFormatSettings { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class Grid
    {
        [JsonPropertyName("show")]
        public bool Show { get; set; } = false;

        [JsonPropertyName("backgroundColor")]
        public string? BackgroundColor { get; set; }

        [JsonPropertyName("borderWidth")]
        public int BorderWidth { get; set; } = 1;

        [JsonPropertyName("borderColor")]
        public string? BorderColor { get; set; } = "#ccc";
    }
}

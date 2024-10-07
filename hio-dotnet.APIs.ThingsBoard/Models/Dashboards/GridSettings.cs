using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class GridSettings
    {
        [JsonPropertyName("backgroundColor")]
        public string? BackgroundColor { get; set; } = "#eeeeee";

        [JsonPropertyName("columns")]
        public int Columns { get; set; } = 24;

        [JsonPropertyName("margin")]
        public int Margin { get; set; } = 10;

        [JsonPropertyName("outerMargin")]
        public bool OuterMargin { get; set; } = true;
    }
}

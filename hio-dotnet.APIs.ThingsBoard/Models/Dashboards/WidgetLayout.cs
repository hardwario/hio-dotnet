using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class WidgetLayout
    {
        [JsonPropertyName("sizeX")]
        public int SizeX { get; set; } = 0;

        [JsonPropertyName("sizeY")]
        public int SizeY { get; set; } = 0;

        [JsonPropertyName("row")]
        public int Row { get; set; } = 0;

        [JsonPropertyName("col")]
        public int Col { get; set; } = 0;
    }
}

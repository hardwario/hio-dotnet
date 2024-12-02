using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class TitleStyle
    {
        [JsonPropertyName("fontSize")]
        public string? FontSize { get; set; } = "16px";

        [JsonPropertyName("fontWeight")]
        public int? FontWeight { get; set; } = 400;
    }
}

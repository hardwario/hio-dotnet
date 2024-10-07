using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class Settings
    {
        [JsonPropertyName("stateControllerId")]
        public string? StateControllerId { get; set; } = "entity";

        [JsonPropertyName("showTitle")]
        public bool ShowTitle { get; set; } = false;
    }
}

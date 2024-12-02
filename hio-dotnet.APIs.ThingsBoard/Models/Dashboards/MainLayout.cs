using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class MainLayout
    {
        [JsonPropertyName("widgets")]
        public Dictionary<string, WidgetLayout> Widgets { get; set; } = new Dictionary<string, WidgetLayout>();

        [JsonPropertyName("gridSettings")]
        public GridSettings GridSettings { get; set; } = new GridSettings();
    }
}

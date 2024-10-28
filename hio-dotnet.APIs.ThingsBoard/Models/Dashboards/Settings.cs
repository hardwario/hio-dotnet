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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? StateControllerId { get; set; }

        [JsonPropertyName("showTitle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowTitle { get; set; }

        [JsonPropertyName("showEntitiesSelect")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowEntitiesSelect { get; set; }

        [JsonPropertyName("showDashboardTimewindow")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowDashboardTimewindow { get; set; }

        [JsonPropertyName("showDashboardExport")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowDashboardExport { get; set; }

        [JsonPropertyName("toolbarAlwaysOpen")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ToolbarAlwaysOpen { get; set; }
    }
}

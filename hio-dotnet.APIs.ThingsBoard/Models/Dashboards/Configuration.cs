using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class Configuration
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; } = string.Empty;

        [JsonPropertyName("widgets")]
        public Dictionary<string, Widget> Widgets { get; set; } = new Dictionary<string, Widget>();

        [JsonPropertyName("states")]
        public States States { get; set; } = new States();

        [JsonPropertyName("entityAliases")]
        public Dictionary<string, EntityAlias>? EntityAliases { get; set; } = new Dictionary<string, EntityAlias>();

        [JsonPropertyName("filters")]
        public Dictionary<string, object>? Filters { get; set; } = new Dictionary<string, object>();

        [JsonPropertyName("timewindow")]
        public Timewindow Timewindow { get; set; } = new Timewindow();

        [JsonPropertyName("settings")]
        public Settings Settings { get; set; } = new Settings();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class Config
    {
        [JsonPropertyName("datasources")]
        public List<Datasource> Datasources { get; set; } = new List<Datasource>();

        [JsonPropertyName("timewindow")]
        public Timewindow Timewindow { get; set; } = new Timewindow();

        [JsonPropertyName("showTitle")]
        public bool ShowTitle { get; set; } = false;

        [JsonPropertyName("backgroundColor")]
        public string? BackgroundColor { get; set; } = "#fff";

        [JsonPropertyName("color")]
        public string? Color { get; set; } = "rgba(0, 0, 0, 0.87)";

        [JsonPropertyName("padding")]
        public string? Padding { get; set; } = "0px";

        [JsonPropertyName("settings")]
        public Settings Settings { get; set; } = new Settings();

        [JsonPropertyName("title")]
        public string? Title { get; set; } = string.Empty;

        [JsonPropertyName("dropShadow")]
        public bool DropShadow { get; set; } = false;

        [JsonPropertyName("enableFullscreen")]
        public bool EnableFullscreen { get; set; } = true;

        [JsonPropertyName("widgetStyle")]
        public Dictionary<string, object>? WidgetStyle { get; set; } = new Dictionary<string, object>();
    }
}

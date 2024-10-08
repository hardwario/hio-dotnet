using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class LineSettings
    {
        [JsonPropertyName("showLine")]
        public bool ShowLine { get; set; } = true;

        [JsonPropertyName("step")]
        public bool Step { get; set; } = false;

        [JsonPropertyName("stepType")]
        public string? StepType { get; set; } = "start";

        [JsonPropertyName("smooth")]
        public bool Smooth { get; set; } = false;

        [JsonPropertyName("lineType")]
        public string? LineType { get; set; } = "solid";

        [JsonPropertyName("lineWidth")]
        public int LineWidth { get; set; } = 2;

        [JsonPropertyName("showPoints")]
        public bool ShowPoints { get; set; } = false;

        [JsonPropertyName("showPointLabel")]
        public bool ShowPointLabel { get; set; } = false;

        [JsonPropertyName("pointLabelPosition")]
        public string? PointLabelPosition { get; set; } = "top";

        [JsonPropertyName("pointLabelFont")]
        public FontSettings PointLabelFont { get; set; } = new FontSettings();

        [JsonPropertyName("pointLabelColor")]
        public string? PointLabelColor { get; set; } = "rgba(0, 0, 0, 0.76)";

        [JsonPropertyName("enablePointLabelBackground")]
        public bool EnablePointLabelBackground { get; set; } = false;

        [JsonPropertyName("pointLabelBackground")]
        public string? PointLabelBackground { get; set; } = "rgba(255,255,255,0.56)";

        [JsonPropertyName("pointShape")]
        public string? PointShape { get; set; } = "emptyCircle";

        [JsonPropertyName("pointSize")]
        public int PointSize { get; set; } = 4;

        [JsonPropertyName("fillAreaSettings")]
        public FillAreaSettings FillAreaSettings { get; set; } = new FillAreaSettings();
    }
}

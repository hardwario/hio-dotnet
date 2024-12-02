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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Datasource>? Datasources { get; set; }

        [JsonPropertyName("timewindow")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Timewindow? Timewindow { get; set; }

        [JsonPropertyName("showTitle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowTitle { get; set; }

        [JsonPropertyName("backgroundColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BackgroundColor { get; set; }

        [JsonPropertyName("color")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Color { get; set; }

        [JsonPropertyName("padding")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Padding { get; set; }

        [JsonPropertyName("settings")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ConfigSettings? Settings { get; set; }

        [JsonPropertyName("title")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Title { get; set; }

        [JsonPropertyName("dropShadow")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? DropShadow { get; set; }

        [JsonPropertyName("enableFullscreen")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? EnableFullscreen { get; set; }

        [JsonPropertyName("configMode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ConfigMode { get; set; }

        [JsonPropertyName("actions")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? Actions { get; set; }

        [JsonPropertyName("showTitleIcon")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowTitleIcon { get; set; }

        [JsonPropertyName("titleIcon")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TitleIcon { get; set; }

        [JsonPropertyName("iconColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? IconColor { get; set; }

        [JsonPropertyName("useDashboardTimewindow")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? UseDashboardTimewindow { get; set; }

        [JsonPropertyName("displayTimewindow")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? DisplayTimewindow { get; set; }

        [JsonPropertyName("titleFont")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FontSettings? TitleFont { get; set; }

        [JsonPropertyName("titleColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TitleColor { get; set; }

        [JsonPropertyName("titleTooltip")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TitleTooltip { get; set; }

        [JsonPropertyName("titleStyle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TitleStyle? TitleStyle { get; set; }

        [JsonPropertyName("widgetStyle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? WidgetStyle { get; set; }

        [JsonPropertyName("widgetCss")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? WidgetCss { get; set; }

        [JsonPropertyName("pageSize")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PageSize { get; set; }

        [JsonPropertyName("units")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Units { get; set; }

        [JsonPropertyName("decimals")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Decimals { get; set; }

        [JsonPropertyName("noDataDisplayMessage")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? NoDataDisplayMessage { get; set; }

        [JsonPropertyName("timewindowStyle")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TimewindowStyle? TimewindowStyle { get; set; }

        [JsonPropertyName("margin")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Margin { get; set; }

        [JsonPropertyName("borderRadius")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BorderRadius { get; set; }

        [JsonPropertyName("iconSize")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? IconSize { get; set; }

        [JsonPropertyName("showLegend")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowLegend { get; set; }
    }
}

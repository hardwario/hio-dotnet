using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class ConfigSettings
    {
        [JsonPropertyName("showLegend")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowLegend { get; set; }

        [JsonPropertyName("legendConfig")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LegendConfig? LegendConfig { get; set; }

        [JsonPropertyName("thresholds")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<object>? Thresholds { get; set; }

        [JsonPropertyName("dataZoom")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? DataZoom { get; set; }

        [JsonPropertyName("stack")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Stack { get; set; }

        [JsonPropertyName("yAxis")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AxisSettings? YAxis { get; set; }

        [JsonPropertyName("xAxis")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AxisSettings? XAxis { get; set; }

        [JsonPropertyName("legendLabelFont")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FontSettings? LegendLabelFont { get; set; }

        [JsonPropertyName("legendLabelColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LegendLabelColor { get; set; }

        [JsonPropertyName("showTooltip")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ShowTooltip { get; set; }

        [JsonPropertyName("tooltipTrigger")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TooltipTrigger { get; set; }

        [JsonPropertyName("tooltipValueFont")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FontSettings? TooltipValueFont { get; set; }

        [JsonPropertyName("tooltipValueColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TooltipValueColor { get; set; }

        [JsonPropertyName("tooltipShowDate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? TooltipShowDate { get; set; }

        [JsonPropertyName("tooltipDateFormat")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TooltipDateFormat? TooltipDateFormat { get; set; }

        [JsonPropertyName("tooltipDateFont")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FontSettings? TooltipDateFont { get; set; }

        [JsonPropertyName("tooltipDateColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TooltipDateColor { get; set; }

        [JsonPropertyName("tooltipDateInterval")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? TooltipDateInterval { get; set; }

        [JsonPropertyName("tooltipBackgroundColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TooltipBackgroundColor { get; set; }

        [JsonPropertyName("tooltipBackgroundBlur")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TooltipBackgroundBlur { get; set; }

        [JsonPropertyName("background")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Background? Background { get; set; }

        [JsonPropertyName("yAxes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, AxisDetails>? YAxes { get; set; }

        [JsonPropertyName("noAggregationBarWidthSettings")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public NoAggregationBarWidthSettings? NoAggregationBarWidthSettings { get; set; }

        [JsonPropertyName("animation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Animation? Animation { get; set; }

        [JsonPropertyName("padding")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Padding { get; set; }

        [JsonPropertyName("comparisonEnabled")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ComparisonEnabled { get; set; }

        [JsonPropertyName("timeForComparison")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TimeForComparison { get; set; }

        [JsonPropertyName("comparisonCustomIntervalValue")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ComparisonCustomIntervalValue { get; set; }

        [JsonPropertyName("comparisonXAxis")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AxisSettings? ComparisonXAxis { get; set; }

        [JsonPropertyName("grid")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Grid? Grid { get; set; }

        [JsonPropertyName("legendColumnTitleFont")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FontSettings? LegendColumnTitleFont { get; set; }

        [JsonPropertyName("legendColumnTitleColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LegendColumnTitleColor { get; set; }

        [JsonPropertyName("legendValueFont")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FontSettings? LegendValueFont { get; set; }

        [JsonPropertyName("legendValueColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? LegendValueColor { get; set; }

        [JsonPropertyName("tooltipLabelFont")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FontSettings? TooltipLabelFont { get; set; }

        [JsonPropertyName("tooltipLabelColor")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TooltipLabelColor { get; set; }

        [JsonPropertyName("cardHtml")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CardHtml { get; set; }

        [JsonPropertyName("cardCss")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CardCss { get; set; }
    }
}

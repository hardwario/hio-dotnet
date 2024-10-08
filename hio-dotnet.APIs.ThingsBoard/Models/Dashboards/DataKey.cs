using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class DataKey
    {
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Type { get; set; }

        [JsonPropertyName("aggregationType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AggregationType { get; set; }

        [JsonPropertyName("label")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Label { get; set; }

        [JsonPropertyName("color")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Color { get; set; }

        [JsonPropertyName("settings")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DataKeySettings? Settings { get; set; }

        [JsonPropertyName("_hash")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Hash { get; set; }

        [JsonPropertyName("units")]
        public string? Units { get; set; }

        [JsonPropertyName("decimals")]
        public int? Decimals { get; set; }

        [JsonPropertyName("funcBody")]
        public string? FuncBody { get; set; }

        [JsonPropertyName("usePostProcessing")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? UsePostProcessing { get; set; } = false;

        [JsonPropertyName("postFuncBody")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PostFuncBody { get; set; }
    }
}

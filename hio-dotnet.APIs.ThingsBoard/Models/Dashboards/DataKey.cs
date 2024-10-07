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
        public string? Name { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string? Type { get; set; } = string.Empty;

        [JsonPropertyName("label")]
        public string? Label { get; set; } = string.Empty;

        [JsonPropertyName("color")]
        public string? Color { get; set; } = string.Empty;

        [JsonPropertyName("settings")]
        public Dictionary<string, object>? Settings { get; set; } = new Dictionary<string, object>();

        [JsonPropertyName("_hash")]
        public double Hash { get; set; } = 0;

        [JsonPropertyName("aggregationType")]
        public string? AggregationType { get; set; } = "NONE";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class Widget
    {
        [JsonPropertyName("typeFullFqn")]
        public string? TypeFullFqn { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string? Type { get; set; } = string.Empty;

        [JsonPropertyName("sizeX")]
        public double SizeX { get; set; } = 0;

        [JsonPropertyName("sizeY")]
        public double SizeY { get; set; } = 0;

        [JsonPropertyName("config")]
        public Config Config { get; set; } = new Config();

        [JsonPropertyName("row")]
        public int Row { get; set; } = 0;

        [JsonPropertyName("col")]
        public int Col { get; set; } = 0;

        [JsonPropertyName("id")]
        public string? Id { get; set; } = string.Empty;
    }
}

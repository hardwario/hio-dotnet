using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class Aggregation
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; } = "AVG";

        [JsonPropertyName("limit")]
        public int Limit { get; set; } = 25000;
    }
}

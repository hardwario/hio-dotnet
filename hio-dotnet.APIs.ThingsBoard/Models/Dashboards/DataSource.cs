using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class Datasource
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string? Name { get; set; } = string.Empty;

        [JsonPropertyName("deviceId")]
        public string? DeviceId { get; set; } = string.Empty;

        [JsonPropertyName("dataKeys")]
        public List<DataKey> DataKeys { get; set; } = new List<DataKey>();
    }
}

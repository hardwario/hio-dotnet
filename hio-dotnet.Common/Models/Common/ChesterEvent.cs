using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class ChesterEvent
    {
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        [JsonPropertyName("type")]
        public string Type { get; set; } = Defaults.UnknownEventType;

        [JsonPropertyName("value")]
        public double Value { get; set; } = 0.0;
    }
}

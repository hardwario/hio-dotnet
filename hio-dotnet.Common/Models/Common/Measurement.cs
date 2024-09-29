using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class Measurement
    {
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        [JsonPropertyName("min")]
        public double Min { get; set; } = 0.0;

        [JsonPropertyName("max")]
        public double Max { get; set; } = 0.0;

        [JsonPropertyName("avg")]
        public double Avg { get; set; } = 0.0;

        [JsonPropertyName("mdn")]
        public double Mdn { get; set; } = 0.0;
    }
}

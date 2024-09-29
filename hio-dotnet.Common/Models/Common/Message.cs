using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class Message
    {
        [JsonPropertyName("version")]
        public int Version { get; set; } = 0;

        [JsonPropertyName("sequence")]
        public int Sequence { get; set; } = 0;

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}

using hio_dotnet.Common.Models.DataSimulation;
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
        [SimulationAttribute(true)]
        [JsonPropertyName("version")]
        public int Version { get; set; } = 0;

        [SimulationAttribute(true)]
        [JsonPropertyName("sequence")]
        public int Sequence { get; set; } = 0;

        [SimulationAttribute(true)]
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}

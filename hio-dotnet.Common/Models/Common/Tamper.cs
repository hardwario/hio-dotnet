using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class Tamper
    {
        [JsonPropertyName("state")]
        public string State { get; set; } = Defaults.UnknownTamperState;

        [JsonPropertyName("events")]
        public List<ChesterBaseEvent> Events { get; set; } = new List<ChesterBaseEvent>();
    }
}

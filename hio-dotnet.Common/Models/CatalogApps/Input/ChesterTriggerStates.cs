using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Input
{
    public class ChesterTriggerStates
    {
        [JsonPropertyName("channel")]
        public int Channel { get; set; } = 0;

        [JsonPropertyName("state")]
        public string State { get; set; } = Defaults.InactiveState;

        [JsonPropertyName("events")]
        public List<ChesterBaseEvent> Events { get; set; } = new List<ChesterBaseEvent>();
    }
}

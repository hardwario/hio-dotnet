using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Input
{
    public class InputCounterStates
    {
        [JsonPropertyName("value")]
        public int Value { get; set; } = 0;

        [JsonPropertyName("channel")]
        public int Channel { get; set; } = 0;

        [JsonPropertyName("delta")]
        public int Delta { get; set; } = 0;

        [JsonPropertyName("measurements")]
        public List<ChesterIntEvent> Measurements { get; set; } = new List<ChesterIntEvent>();
    }
}

using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Push
{
    public class PushButtonsStates
    {
        [JsonPropertyName("count_click")]
        public int CountClick { get; set; } = 0;
        
        [JsonPropertyName("count_hold")]
        public int CountHold { get; set; } = 0;

        [JsonPropertyName("events")]
        public List<ChesterBaseEvent> Events { get; set; } = new List<ChesterBaseEvent>();
    }
}

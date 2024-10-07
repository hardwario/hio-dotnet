using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class ClientSettings
    {
        [JsonPropertyName("powerMode")]
        public string PowerMode { get; set; } = string.Empty;
        [JsonPropertyName("psmActivityTimer")]
        public long PsmActivityTimer { get; set; }
        [JsonPropertyName("edrxCycle")]
        public long EdrxCycle { get; set; }
        [JsonPropertyName("pagingTransmissionWindow")]
        public long PagingTransmissionWindow { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class DynamicValue
    {
        [JsonPropertyName("sourceType")]
        public string SourceType { get; set; } = string.Empty;
        [JsonPropertyName("sourceAttribute")]
        public string SourceAttribute { get; set; } = string.Empty;
        [JsonPropertyName("inherit")]
        public bool Inherit { get; set; } = false;
    }
}

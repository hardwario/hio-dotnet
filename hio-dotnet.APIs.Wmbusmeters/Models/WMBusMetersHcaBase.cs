using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.Wmbusmeters.Models
{
    public class WMBusMetersHcaBase : WMBusMetersCommon
    {
        [JsonPropertyName("current_hca")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? CurrentHca { get; set; }
        
        [JsonPropertyName("previous_hca")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? PreviousHca { get; set; }
    }
}

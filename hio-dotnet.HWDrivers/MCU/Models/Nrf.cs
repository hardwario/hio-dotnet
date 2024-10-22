using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.MCU.Models
{
    public class Nrf
    {
        [JsonPropertyName("revision")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Revision { get; set; }
    }
}

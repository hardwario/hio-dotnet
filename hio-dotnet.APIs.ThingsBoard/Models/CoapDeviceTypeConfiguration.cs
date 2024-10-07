using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class CoapDeviceTypeConfiguration
    {
        [JsonPropertyName("coapDeviceType")]
        public string CoapDeviceType { get; set; } = string.Empty;
    }
}

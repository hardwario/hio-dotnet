using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models
{
    public class Firmware
    {
        [JsonPropertyName("zephyr")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Zephyr? Zephyr { get; set; }

        [JsonPropertyName("nrf")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Nrf? Nrf { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class Value
    {
        [JsonPropertyName("defaultValue")]
        public bool DefaultValue { get; set; } = false;
        [JsonPropertyName("userValue")]
        public bool UserValue { get; set; } = false;
        [JsonPropertyName("dynamicValue")]
        public DynamicValue DynamicValue { get; set; } = new DynamicValue();
    }
}

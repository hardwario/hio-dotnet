using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class DataValue
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.MinValue;
        [JsonPropertyName("value")]
        public double Value { get; set; } = 0;
    }
}

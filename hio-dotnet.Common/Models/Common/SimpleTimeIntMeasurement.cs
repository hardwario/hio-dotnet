using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class SimpleTimeIntMeasurement : TimestampState
    {
        [JsonPropertyName("value")]
        public int Value { get; set; } = 0;
    }
}

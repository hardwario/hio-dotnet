using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class TimeStampData
    {
        [System.Text.Json.Serialization.JsonPropertyName("ts")]
        public long Timestamp { get; set; } = 0;

        [System.Text.Json.Serialization.JsonPropertyName("values")]
        public Dictionary<string, object> Values { get; set; } = new Dictionary<string, object>();

        [System.Text.Json.Serialization.JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class ChesterBaseEvent : TimestampState
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = Defaults.UnknownEventType;
    }
}

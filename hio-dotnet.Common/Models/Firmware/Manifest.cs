using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models
{
    public class Manifest
    {
        [JsonPropertyName("format-version")]
        public int? FormatVersion { get; set; }

        [JsonPropertyName("time")]
        public long? Time { get; set; }

        [JsonPropertyName("files")]
        public List<FwFile>? Files { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("firmware")]
        public Firmware? Firmware { get; set; }
    }
}

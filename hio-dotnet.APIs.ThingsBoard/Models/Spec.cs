using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class Spec
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("unit")]
        public string Unit { get; set; } = string.Empty;
        [JsonPropertyName("predicate")]
        public Predicate Predicate { get; set; } = new Predicate();
    }
}

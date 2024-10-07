using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class ConditionDetail
    {
        [JsonPropertyName("key")]
        public Key Key { get; set; } = new Key();
        [JsonPropertyName("valueType")]
        public string ValueType { get; set; } = string.Empty;
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
        [JsonPropertyName("predicate")]
        public Predicate Predicate { get; set; } = new Predicate();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class Alarm
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("alarmType")]
        public string AlarmType { get; set; } = string.Empty;
        [JsonPropertyName("createRules")]
        public Dictionary<string, CreateRule> CreateRules { get; set; } = new Dictionary<string, CreateRule>();
        [JsonPropertyName("clearRule")]
        public ClearRule ClearRule { get; set; } = new ClearRule();
        [JsonPropertyName("propagate")]
        public bool Propagate { get; set; } = false;
        [JsonPropertyName("propagateToOwner")]
        public bool PropagateToOwner { get; set; } = false;
        [JsonPropertyName("propagateToTenant")]
        public bool PropagateToTenant { get; set; } = false;
        [JsonPropertyName("propagateRelationTypes")]
        public List<string> PropagateRelationTypes { get; set; } = new List<string>();
    }
}
